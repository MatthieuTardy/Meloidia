using UnityEngine;

public class AutoShaker : MonoBehaviour
{
    [Header("Paramètres du Tremblement")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    [Header("Scripts à surveiller")]
    public ProgressEnigmeSystem progressEnigmeSystem;
    public MoveToTarget moveSystem;
    public EnigmeSystem enigmeSystemSimple;

    private float currentShakeTime = 0f;
    private Vector3 currentShakeOffset = Vector3.zero;

    private float previousRatio = 0f;

    void Start()
    {
        if (progressEnigmeSystem != null)
        {
            previousRatio = progressEnigmeSystem.ratio;
        }
    }

    void Update()
    {
        if (progressEnigmeSystem != null)
        {
            // Si le ratio augmente = UNE BONNE NOTE A ÉTÉ JOUÉE
            if (progressEnigmeSystem.ratio > previousRatio)
            {
                TriggerShake();

                // On lance le mouvement uniquement sur une bonne note
                if (moveSystem != null)
                {
                    moveSystem.StartMoving();
                }
            }

            previousRatio = progressEnigmeSystem.ratio;
        }

        if (currentShakeTime > 0)
        {
            currentShakeTime -= Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        transform.position -= currentShakeOffset;

        if (currentShakeTime > 0)
        {
            currentShakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeMagnitude,
                Random.Range(-1f, 1f) * shakeMagnitude,
                Random.Range(-1f, 1f) * shakeMagnitude
            );
        }
        else
        {
            currentShakeOffset = Vector3.zero;
        }

        transform.position += currentShakeOffset;
    }

    public void TriggerShake()
    {
        currentShakeTime = shakeDuration;
    }
}