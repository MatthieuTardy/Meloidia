using UnityEngine;

public class AnimatedEnigmaProp : MonoBehaviour
{
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

    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 startScale;

    private Vector3 currentBasePos;
    private Quaternion currentBaseRot;
    private Vector3 currentBaseScale;

    private float previousRatio = 0f;
    private float effectTimeLeft = 0f;

    private float currentBump = 0f;
    private bool isFullyResolved = false;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        startScale = transform.localScale;

        currentBasePos = startPos;
        currentBaseRot = startRot;
        currentBaseScale = startScale;

        if (enigmeSystem != null)
        {
            previousRatio = enigmeSystem.ratio;
        }
    }

    void Update()
    {
        if (enigmeSystem == null) return;

        if (enigmeSystem.ratio > previousRatio)
        {
            if (enigmeSystem.ratio >= 0.99f)
            {
                isFullyResolved = true;
                effectTimeLeft = 0f;
            }
            else
            {
                effectTimeLeft = effectDuration;
                currentBump = bumpPercentage;
            }
        }
        else if (enigmeSystem.ratio == 0 && previousRatio > 0)
        {
            isFullyResolved = false;
            currentBump = 0f;
        }

        previousRatio = enigmeSystem.ratio;

        Vector3 targetPos = startPos;
        Quaternion targetRot = startRot;
        Vector3 targetScale = startScale;

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

        currentBasePos = Vector3.Lerp(currentBasePos, targetPos, moveSpeed * Time.deltaTime);
        currentBaseRot = Quaternion.Lerp(currentBaseRot, targetRot, moveSpeed * Time.deltaTime);
        currentBaseScale = Vector3.Lerp(currentBaseScale, targetScale, moveSpeed * Time.deltaTime);

        Vector3 posOffset = Vector3.zero;
        Quaternion rotOffset = Quaternion.identity;

        if (effectTimeLeft > 0)
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
}