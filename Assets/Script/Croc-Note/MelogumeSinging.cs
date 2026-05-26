using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MelogumeSingingManager : MonoBehaviour
{
    [Header("SingPattern")]
    [SerializeField] musicalNotes[] DefaultPattern;
    [SerializeField] float DefaultSpeed;
    [SerializeField] musicalNotes[] HappyPattern;
    [SerializeField] float HappySpeed;
    [SerializeField] musicalNotes[] AngryPattern;
    [SerializeField] float AngrySpeed;
    [SerializeField] musicalNotes[] SadPattern;
    [SerializeField] float SadSpeed;
    musicalNotes[] currentSingPattern;

    [Header("Effets de Particules")]
    public GameObject noteParticlePrefab;
    public Transform particleSpawnPoint;
    public float noteSpeed;
    [SerializeField] CustomNoteDictionary[] CustomDictionary;
    public Coroutine SingRoutine;
    private bool _isGameManagerReady = false;
    [SerializeField] LegumeManager legumeManager;
    int OldNotePlayer = -1;



    void Start()
    {
        legumeManager = GetComponent<LegumeManager>();

        if (CustomDictionary.Length != 8)
        {
            Debug.LogError("Attention : Des notes manques dans le CustomDictionary ne sont pas assign�s dans l'Inspecteur du GameObject " + gameObject.name + ". La chanson ne d�marrera pas.");
            return;
        }

        if (GameManager.Instance != null && legumeManager != null)
        {
            _isGameManagerReady = true;
        }
        currentSingPattern = DefaultPattern;
        noteSpeed = 1f;
        SingRoutine = StartCoroutine(SingPattern(currentSingPattern));
    }

    private void Update()
    {
        FollowPlayerNote();
    }

    // Arr�te tous les sons jou�s par ce script
    void StopChant()
    {
        foreach(var note in CustomDictionary)
        {
            if (note.Emitter)
            {
                note.Emitter.Stop();
            }
        }
    }

    /// <summary>
    /// Joue une note et d�clenche l'effet de particules associ�.
    /// </summary>
    /// <param name="noteEmitter">L'�metteur FMOD de la note � jouer.</param>
    /// <param name="particleMaterial">Le mat�riau � appliquer aux particules.</param>
    void PlayNoteWithParticles(FMODUnity.StudioEventEmitter noteEmitter, Material particleMaterial)
    {
        // 1. Jouer le son
        noteEmitter.Play();

        // 2. Cr�er les particules si tout est configur�
        if (noteParticlePrefab != null && particleMaterial != null)
        {

            // D�termine la position et la rotation
            Vector3 spawnPosition = particleSpawnPoint != null ? particleSpawnPoint.position : transform.position;
            // Les particules sont orient�es dans la m�me direction que le GameObject
            Quaternion spawnRotation = transform.rotation;

            GameObject particleInstance = Instantiate(noteParticlePrefab, spawnPosition, spawnRotation);
            ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                // Applique le bon mat�riau
                var renderer = ps.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.material = particleMaterial;
                }
                // D�truit l'objet apr�s la fin de l'effet
                Destroy(particleInstance, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(particleInstance, 5f); // S�curit�
            }
        }
    }

    public IEnumerator SingPattern(musicalNotes[] pattern)
    {
        // G�rer la vitesse uniquement si la r�f�rence GameManager est pr�te
        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 0;

        }
        if(legumeManager.CurrentTarget != null)
        {
            yield return new WaitUntil(() => legumeManager.CurrentTarget == null);
        }

        if (legumeManager.isTeleporting)
        {
            yield return new WaitUntil(() => legumeManager.isTeleporting == false);

        }
        // --- S�quence musicale avec particules ---
        legumeManager.animator.SetBool("walk", false);
        legumeManager.animator.SetBool("sing", true);

        foreach(var note in pattern)
        {
            foreach(var dic in CustomDictionary)
            {
                if(dic.ID == note)
                {
                    PlayNoteWithParticles(dic.Emitter, dic.Mat);
                    yield return new WaitForSeconds(noteSpeed);
                    dic.Emitter.Stop();
                }
            }
        }
        legumeManager.animator.SetBool("sing", false);
        // R�tablir la vitesse de d�placement

        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 5;
        }

        // Attente al�atoire avant de r�p�ter la chanson
        yield return new WaitForSeconds(Random.Range(3f, 10.0f));

        // R�p�ter la chanson
        SingRoutine = StartCoroutine(SingPattern(currentSingPattern));
    }
    public void StopHappyness()
    {
        legumeManager.animator.SetBool("sing", false);
    }

    public void StartHappyness()
    {
        noteSpeed = HappySpeed;
        currentSingPattern = HappyPattern;
    }

    public void StartSadness()
    {
        noteSpeed = SadSpeed;
        currentSingPattern = SadPattern;
    }
    public void StartNormal()
    {
        noteSpeed = 1f;
        currentSingPattern = DefaultPattern;
    }
    /*
    public IEnumerator SongOfRage()
    {
        // G�rer la vitesse uniquement si la r�f�rence GameManager est pr�te
        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 0;
        }


        // --- S�quence musicale avec particules ---
        legumeManager.animator.SetBool("walk", false);
        legumeManager.animator.SetBool("sing", true);
        PlayNoteWithParticles(DO, doMaterial);
        yield return new WaitForSeconds(0.1f);
        StopChant();
        PlayNoteWithParticles(DO, doMaterial);
        yield return new WaitForSeconds(0.1f);
        StopChant();
        PlayNoteWithParticles(DO, doMaterial);
        yield return new WaitForSeconds(0.1f);
        StopChant();
        PlayNoteWithParticles(DO, doMaterial);
        yield return new WaitForSeconds(0.3f);
        StopChant();
        PlayNoteWithParticles(DO, doMaterial);
        yield return new WaitForSeconds(0.1f);
        StopChant();
        PlayNoteWithParticles(DO, doMaterial);
        yield return new WaitForSeconds(0.1f);
        StopChant();
        PlayNoteWithParticles(DO, doMaterial);
        yield return new WaitForSeconds(0.3f);
        StopChant();
        PlayNoteWithParticles(DO, doMaterial);
        yield return new WaitForSeconds(0.3f);
        StopChant();
        PlayNoteWithParticles(DO, doMaterial);
        yield return new WaitForSeconds(0.3f);
        StopChant();
        legumeManager.animator.SetBool("sing", false);


        // R�tablir la vitesse de d�placement
        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 5;
        }

        // Attente al�atoire avant de r�p�ter la chanson
        yield return new WaitForSeconds(Random.Range(3f, 10.0f));

        // R�p�ter la chanson
        rage = StartCoroutine(SongOfRage());
    }
    */
    public void StartRage()
    {
        legumeManager.animator.SetBool("sing", false);
        noteSpeed = 0.1f;
        currentSingPattern = AngryPattern;
    }
    public void StopRage()
    {
        //StopCoroutine(rage);
    }


    void FollowPlayerNote()
    {
        if (legumeManager.CurrentTarget != null)
        {
            if(GameManager.Instance.playerManager.noteSystem.playedPartition.Count > 0)
            {
                int notePlayer = GameManager.Instance.playerManager.noteSystem.GetLastNoteIndex();
                if (notePlayer >= 0)
                {
                    if(notePlayer != OldNotePlayer)
                    PlayNoteWithParticles(CustomDictionary[notePlayer].Emitter, CustomDictionary[notePlayer].Mat);
                    StartCoroutine(StopEmitterAfterTime(.5f,CustomDictionary[notePlayer].Emitter));
                    OldNotePlayer = GameManager.Instance.playerManager.noteSystem.GetLastNoteIndex();
                }
            }
        }
    }

    IEnumerator StopEmitterAfterTime(float time,FMODUnity.StudioEventEmitter emitter)
    {
        yield return new WaitForSeconds(time);
        emitter.Stop();
    }
}

[System.Serializable]
class CustomNoteDictionary
{
    public musicalNotes ID;
    public FMODUnity.StudioEventEmitter Emitter;
    public Material Mat;
}
