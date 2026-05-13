using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class AnimatedEnigmaProp : MonoBehaviour
{
    [Header("Link to Enigma (Optional)")]
    public ProgressEnigmeSystem enigmeSystem;

    [Header("Target State (Optional)")]
    public Transform finalTarget;
    public float moveSpeed = 5f;
    public float fallBackSpeed = 2f;
    [Range(0f, 1f)] public float bumpPercentage = 0.2f;

    [Header("Reaction Effect (Shake / Sway)")]
    public float effectDuration = 0.3f;
    public Vector3 positionalShake = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 rotationalShake = new Vector3(0f, 0f, 0f);

    [Header("Cinematic Camera (Focus)")]
    public bool enableCinematicCamera = true;
    public Transform cinematicCameraPoint;
    public float cameraFocusDuration = 2f;

    [Header("Camera Shake Settings")]
    public bool enableCameraShake = false;
    [Tooltip("Clique sur le petit point à droite pour choisir un profil comme '6D Shake'")]
    public NoiseSettings cameraNoiseProfile;
    public float shakeAmplitude = 1.5f;
    public float shakeFrequency = 2.0f;

    [Header("Chaining Events")]
    public UnityEvent onEnigmaSolvedImmediate;
    public UnityEvent onCameraSequenceEnd;

    private CinemachineVirtualCamera sequenceCam;
    private bool hasTriggeredCamera = false;

    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 startScale;

    private Vector3 currentBasePos;
    private Quaternion currentBaseRot;
    private Vector3 currentBaseScale;

    private Vector3 targetPos;
    private Quaternion targetRot;
    private Vector3 targetScale;

    private float previousRatio = 0f;
    private float effectTimeLeft = 0f;
    private float currentBump = 0f;
    private bool isFullyResolved = false;

    void Start()
    {
        InitializeTransforms();

        if (enigmeSystem != null)
        {
            previousRatio = enigmeSystem.ratio;
        }
    }

    void Update()
    {
        CheckEnigmaProgress();
        CalculateTargetTransform();
        ApplyMovementAndEffects();
    }

    private void InitializeTransforms()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        startScale = transform.localScale;

        currentBasePos = startPos;
        currentBaseRot = startRot;
        currentBaseScale = startScale;

        targetPos = startPos;
        targetRot = startRot;
        targetScale = startScale;
    }

    private void CheckEnigmaProgress()
    {
        if (enigmeSystem == null || Mathf.Approximately(enigmeSystem.ratio, previousRatio))
            return;

        if (enigmeSystem.ratio == 0f)
        {
            ResetToStart();
        }
        else if (enigmeSystem.ratio >= 0.99f)
        {
            HandleEnigmaResolved();
        }
        else
        {
            HandleEnigmaProgressing();
        }

        previousRatio = enigmeSystem.ratio;
    }

    private void HandleEnigmaResolved()
    {
        isFullyResolved = true;
        effectTimeLeft = 0f;

        if (!hasTriggeredCamera)
        {
            onEnigmaSolvedImmediate?.Invoke();
            TriggerCameraFocus();
        }
    }

    private void HandleEnigmaProgressing()
    {
        isFullyResolved = false;
        effectTimeLeft = effectDuration;
        currentBump = bumpPercentage;
    }

    private void CalculateTargetTransform()
    {
        targetPos = startPos;
        targetRot = startRot;
        targetScale = startScale;

        if (finalTarget != null)
        {
            if (isFullyResolved)
            {
                targetPos = finalTarget.position;
                targetRot = finalTarget.rotation;
                targetScale = finalTarget.localScale;
            }
            else
            {
                currentBump = Mathf.Lerp(currentBump, 0f, fallBackSpeed * Time.deltaTime);
                targetPos = Vector3.Lerp(startPos, finalTarget.position, currentBump);
                targetRot = Quaternion.Lerp(startRot, finalTarget.rotation, currentBump);
                targetScale = Vector3.Lerp(startScale, finalTarget.localScale, currentBump);
            }
        }
    }

    private void ApplyMovementAndEffects()
    {
        currentBasePos = Vector3.Lerp(currentBasePos, targetPos, moveSpeed * Time.deltaTime);
        currentBaseRot = Quaternion.Lerp(currentBaseRot, targetRot, moveSpeed * Time.deltaTime);
        currentBaseScale = Vector3.Lerp(currentBaseScale, targetScale, moveSpeed * Time.deltaTime);

        Vector3 posOffset = Vector3.zero;
        Quaternion rotOffset = Quaternion.identity;

        if (effectTimeLeft > 0 && !isFullyResolved)
        {
            posOffset = new Vector3(
                Random.Range(-1f, 1f) * positionalShake.x,
                Random.Range(-1f, 1f) * positionalShake.y,
                Random.Range(-1f, 1f) * positionalShake.z
            );

            Vector3 eulerShake = new Vector3(
                Random.Range(-1f, 1f) * rotationalShake.x,
                Random.Range(-1f, 1f) * rotationalShake.y,
                Random.Range(-1f, 1f) * rotationalShake.z
            );
            rotOffset = Quaternion.Euler(eulerShake);

            effectTimeLeft -= Time.deltaTime;
        }

        transform.position = currentBasePos + posOffset;
        transform.rotation = currentBaseRot * rotOffset;
        transform.localScale = currentBaseScale;
    }

    private void TriggerCameraFocus()
    {
        if (enableCinematicCamera && cinematicCameraPoint != null)
        {
            hasTriggeredCamera = true;
            StartCoroutine(CameraFocusRoutine());
        }
        else if (!hasTriggeredCamera)
        {
            hasTriggeredCamera = true;
            onCameraSequenceEnd?.Invoke();
        }
    }

    private IEnumerator CameraFocusRoutine()
    {
        GameObject camObj = new GameObject("EnigmaProp_CinematicCam");
        camObj.transform.position = cinematicCameraPoint.position;

        sequenceCam = camObj.AddComponent<CinemachineVirtualCamera>();

        sequenceCam.Follow = cinematicCameraPoint;
        sequenceCam.LookAt = this.transform;
        sequenceCam.Priority = 100;

        var transposer = sequenceCam.AddCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset = Vector3.zero;

        var composer = sequenceCam.AddCinemachineComponent<CinemachineComposer>();
        composer.m_TrackedObjectOffset = new Vector3(0, 0f, 0);

        if (enableCameraShake)
        {
            if (cameraNoiseProfile != null)
            {
                var noise = sequenceCam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                noise.m_NoiseProfile = cameraNoiseProfile;
                noise.m_AmplitudeGain = shakeAmplitude;
                noise.m_FrequencyGain = shakeFrequency;
            }
            else
            {
                Debug.LogWarning($"Camera Shake est activé sur {gameObject.name} mais il manque le fichier dans 'Camera Noise Profile' !");
            }
        }

        yield return new WaitForSeconds(cameraFocusDuration);

        if (sequenceCam != null)
        {
            Destroy(sequenceCam.gameObject);
        }

        onCameraSequenceEnd?.Invoke();
    }

    public void PlayAnimation()
    {
        isFullyResolved = true;
        effectTimeLeft = 0f;

        if (!hasTriggeredCamera)
        {
            onEnigmaSolvedImmediate?.Invoke();
            TriggerCameraFocus();
        }
    }

    public void ResetToStart()
    {
        isFullyResolved = false;
        hasTriggeredCamera = false;
        effectTimeLeft = 0f;
        currentBump = 0f;
    }
}