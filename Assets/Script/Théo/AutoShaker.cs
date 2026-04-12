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

    // Variables pour gérer le mouvement basé sur le ratio
    private Vector3 initialMovePos;
    private Quaternion initialMoveRot;
    private Vector3 initialMoveScale;
    private bool hasCapturedInitialMoveState = false;

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
            // Initialisation de la position de départ pour MoveToTarget
            if (moveSystem != null && !hasCapturedInitialMoveState)
            {
                initialMovePos = moveSystem.transform.position;
                initialMoveRot = moveSystem.transform.rotation;
                initialMoveScale = moveSystem.transform.localScale;
                hasCapturedInitialMoveState = true;
            }

            // Si le ratio augmente = UNE BONNE NOTE A ÉTÉ JOUÉE
            if (progressEnigmeSystem.ratio > previousRatio)
            {
                TriggerShake();
            }

            // On met à jour la position du MoveToTarget en fonction du ratio d'avancement
            if (moveSystem != null && moveSystem.target != null && hasCapturedInitialMoveState)
            {
                // Si on a fait une erreur (le ratio retombe à 0)
                if (progressEnigmeSystem.ratio == 0 && previousRatio > 0)
                {
                    // Optionnel : tu pourrais rajouter un "mauvais shake" ici
                }

                // Déplacement vers la cible au pourcentage exact des bonnes notes validées
                moveSystem.transform.position = Vector3.Lerp(initialMovePos, moveSystem.target.position, progressEnigmeSystem.ratio);
                moveSystem.transform.rotation = Quaternion.Slerp(initialMoveRot, moveSystem.target.rotation, progressEnigmeSystem.ratio);
                moveSystem.transform.localScale = Vector3.Lerp(initialMoveScale, moveSystem.target.localScale, progressEnigmeSystem.ratio);
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