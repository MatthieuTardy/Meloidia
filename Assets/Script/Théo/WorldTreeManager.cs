using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

[RequireComponent(typeof(SphereCollider))]
public class WorldTreeManager : MonoBehaviour
{
    [Header("Growth Stages")]
    public GameObject[] treeStages;
    public int currentStage = 0;

    [Header("Cinematic")]
    public Transform cinematicCameraPoint;
    public ParticleSystem growthParticles;
    public float waitBeforeGrowth = 1f;
    public float waitAfterGrowth = 2.5f;
    public GameObject objectToDeactivateOnStart;

    [Header("Juice / Animation")]
    public float growthDuration = 1.5f;
    public AnimationCurve growthCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    private Vector3[] originalScales;

    [Header("Singing System")]
    public musicalNotes[] finalMelodyPattern;
    public float noteSpeed = 1f;
    public float timeBetweenLoops = 4f;
    public GameObject noteParticlePrefab;
    public Transform[] stageParticleSpawnPoints;

    [Tooltip("Place ici l'Empty GameObject d'où sortiront les particules lors de la mélodie finale")]
    public Transform finalPhaseParticleSpawnPoint; // <-- NOUVELLE VARIABLE ICI

    public WorldTreeNoteDictionary[] customDictionary;

    [Header("Events")]
    public UnityEvent onFirstStageCameraFinished;
    public UnityEvent onGameFinished;

    private CinemachineVirtualCamera sequenceCam;
    private bool isWaitingForPlayerMelody = false;
    private Coroutine singRoutine;
    private Coroutine playerListeningCoroutine;
    private int currentStep = 0;
    private bool isCinematicPlaying = false;

#if UNITY_EDITOR
    [HideInInspector] public Vector3 originalCamPos;
    [HideInInspector] public Quaternion originalCamRot;
    [HideInInspector] public bool isPreviewing = false;
#endif

    private void Reset()
    {
        if (objectToDeactivateOnStart == null)
        {
            objectToDeactivateOnStart = GameObject.Find("Wheel");
        }
    }

    void Start()
    {
        if (objectToDeactivateOnStart == null)
        {
            objectToDeactivateOnStart = GameObject.Find("Wheel");
        }

        GetComponent<SphereCollider>().isTrigger = true;

        originalScales = new Vector3[treeStages.Length];
        for (int i = 0; i < treeStages.Length; i++)
        {
            if (treeStages[i] != null)
            {
                originalScales[i] = treeStages[i].transform.localScale;
            }
        }

        UpdateTreeVisuals(true);
    }

    void Update()
    {
        if (objectToDeactivateOnStart == null)
        {
            objectToDeactivateOnStart = GameObject.Find("Wheel");
        }

        if (isCinematicPlaying && objectToDeactivateOnStart != null && objectToDeactivateOnStart.activeInHierarchy)
        {
            objectToDeactivateOnStart.SetActive(false);
        }
    }

    public void OnRegularTreeCompleted()
    {
        StartCoroutine(GrowthSequence());
    }

    private IEnumerator GrowthSequence()
    {
        isCinematicPlaying = true;
        int startingStage = currentStage;

        if (objectToDeactivateOnStart != null)
        {
            objectToDeactivateOnStart.SetActive(false);
        }

        if (cinematicCameraPoint != null)
        {
            GameObject camObj = new GameObject("WorldTree_CinematicCam");
            camObj.transform.position = cinematicCameraPoint.position;

            sequenceCam = camObj.AddComponent<CinemachineVirtualCamera>();
            sequenceCam.Follow = cinematicCameraPoint;
            sequenceCam.LookAt = this.transform;
            sequenceCam.Priority = 100;

            var transposer = sequenceCam.AddCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = Vector3.zero;

            var composer = sequenceCam.AddCinemachineComponent<CinemachineComposer>();
            composer.m_TrackedObjectOffset = Vector3.zero;
        }

        yield return new WaitForSeconds(waitBeforeGrowth);

        if (currentStage < treeStages.Length - 1)
        {
            int oldStage = currentStage;
            currentStage++;

            if (growthParticles != null)
                growthParticles.Play();

            yield return StartCoroutine(AnimateTreeTransition(oldStage, currentStage));
        }

        yield return new WaitForSeconds(waitAfterGrowth);

        if (sequenceCam != null)
        {
            Destroy(sequenceCam.gameObject);
        }

        isCinematicPlaying = false;

        if (startingStage == 0 && currentStage == 1)
        {
            onFirstStageCameraFinished?.Invoke();
        }

        if (currentStage == treeStages.Length - 1 && !isWaitingForPlayerMelody)
        {
            StartFinalMelodyPhase();
        }
    }

    private IEnumerator AnimateTreeTransition(int oldIndex, int newIndex)
    {
        GameObject oldTree = treeStages[oldIndex];
        GameObject newTree = treeStages[newIndex];

        Vector3 oldStartScale = originalScales[oldIndex];
        Vector3 newTargetScale = originalScales[newIndex];

        if (oldTree != null)
        {
            oldTree.SetActive(false);
            oldTree.transform.localScale = oldStartScale;
        }

        if (newTree != null)
        {
            newTree.SetActive(true);
            newTree.transform.localScale = oldStartScale;
        }

        float time = 0f;
        while (time < growthDuration)
        {
            time += Time.deltaTime;
            float t = time / growthDuration;
            float curveValue = growthCurve.Evaluate(t);

            if (newTree != null)
            {
                newTree.transform.localScale = Vector3.LerpUnclamped(oldStartScale, newTargetScale, curveValue);
            }

            yield return null;
        }

        if (newTree != null) newTree.transform.localScale = newTargetScale;
    }

    private void UpdateTreeVisuals(bool instant)
    {
        for (int i = 0; i < treeStages.Length; i++)
        {
            if (treeStages[i] != null)
            {
                bool isActive = (i == currentStage);
                treeStages[i].SetActive(isActive);

                if (instant && isActive)
                {
                    treeStages[i].transform.localScale = originalScales[i];
                }
            }
        }
    }

    private void StartFinalMelodyPhase()
    {
        isWaitingForPlayerMelody = true;
        singRoutine = StartCoroutine(SingPattern());
    }

    private IEnumerator SingPattern()
    {
        while (isWaitingForPlayerMelody)
        {
            foreach (var note in finalMelodyPattern)
            {
                if (!isWaitingForPlayerMelody) break;

                foreach (var dic in customDictionary)
                {
                    if (dic.ID == note)
                    {
                        PlayNoteWithParticles(dic.Emitter, dic.Mat);
                        yield return new WaitForSeconds(noteSpeed);
                        dic.Emitter.Stop();
                    }
                }
            }
            yield return new WaitForSeconds(timeBetweenLoops);
        }
    }

    private void PlayNoteWithParticles(FMODUnity.StudioEventEmitter noteEmitter, Material particleMaterial)
    {
        noteEmitter.Play();

        if (noteParticlePrefab != null && particleMaterial != null)
        {
            Vector3 spawnPosition = transform.position;

            // --- NOUVELLE LOGIQUE ICI ---
            // On vérifie d'abord si on est dans la phase finale ET qu'un point spécifique a été assigné
            if (isWaitingForPlayerMelody && finalPhaseParticleSpawnPoint != null)
            {
                spawnPosition = finalPhaseParticleSpawnPoint.position;
            }
            // Sinon, on retombe sur l'ancien système (le tableau) s'il est configuré
            else if (stageParticleSpawnPoints != null && currentStage < stageParticleSpawnPoints.Length && stageParticleSpawnPoints[currentStage] != null)
            {
                spawnPosition = stageParticleSpawnPoints[currentStage].position;
            }

            GameObject particleInstance = Instantiate(noteParticlePrefab, spawnPosition, transform.rotation);
            ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                var renderer = ps.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.material = particleMaterial;
                }
                Destroy(particleInstance, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(particleInstance, 5f);
            }
        }
    }

    private void StopAllNotes()
    {
        foreach (var dic in customDictionary)
        {
            if (dic.Emitter != null)
            {
                dic.Emitter.Stop();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isWaitingForPlayerMelody && other.gameObject.layer == 8 && playerListeningCoroutine == null)
        {
            GameManager.Instance.playerManager.noteSystem.ClearPartition();
            playerListeningCoroutine = StartCoroutine(PlayerMelodyLogic());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8 && playerListeningCoroutine != null)
        {
            StopCoroutine(playerListeningCoroutine);
            playerListeningCoroutine = null;
            currentStep = 0;
        }
    }

    private IEnumerator PlayerMelodyLogic()
    {
        int totalNotes = finalMelodyPattern.Length;
        currentStep = 0;

        var noteSystem = GameManager.Instance.playerManager.noteSystem;
        int lastNoteCount = noteSystem.playedPartition.Count;

        while (currentStep < totalNotes && isWaitingForPlayerMelody)
        {
            if (noteSystem.playedPartition.Count > 0)
            {
                musicalNotes expectedNote = finalMelodyPattern[currentStep];

                yield return new WaitUntil(() => noteSystem.playedPartition.Count > lastNoteCount || !isWaitingForPlayerMelody);

                if (!isWaitingForPlayerMelody) break;

                lastNoteCount = noteSystem.playedPartition.Count;

                if (noteSystem.HasJustPlayed(expectedNote))
                {
                    currentStep++;
                }
                else
                {
                    if (totalNotes > 1)
                    {
                        currentStep = 0;
                    }
                }

                if (currentStep >= totalNotes)
                {
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (currentStep >= totalNotes)
        {
            TriggerFinalGameEnd();
        }

        playerListeningCoroutine = null;
    }

    private void TriggerFinalGameEnd()
    {
        isWaitingForPlayerMelody = false;
        isCinematicPlaying = true;

        if (objectToDeactivateOnStart != null)
        {
            objectToDeactivateOnStart.SetActive(false);
        }

        if (singRoutine != null)
        {
            StopCoroutine(singRoutine);
        }

        StopAllNotes();
        onGameFinished?.Invoke();
    }

#if UNITY_EDITOR
    public void TogglePreview()
    {
        GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCam == null)
        {
            Debug.LogWarning("No MainCamera");
            return;
        }

        if (!isPreviewing)
        {
            if (cinematicCameraPoint != null)
            {
                originalCamPos = mainCam.transform.position;
                originalCamRot = mainCam.transform.rotation;

                mainCam.transform.position = cinematicCameraPoint.position;
                mainCam.transform.LookAt(transform);
                isPreviewing = true;
            }
        }
        else
        {
            mainCam.transform.position = originalCamPos;
            mainCam.transform.rotation = originalCamRot;
            isPreviewing = false;
        }
    }
#endif
}

[System.Serializable]
public class WorldTreeNoteDictionary
{
    public musicalNotes ID;
    public FMODUnity.StudioEventEmitter Emitter;
    public Material Mat;
}