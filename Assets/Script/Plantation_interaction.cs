using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plantation_interaction : MonoBehaviour
{
    private bool detected = false;
    private bool planted = false;
    private bool finished = false;
    private bool isHarvesting = false;
    public GameObject plante;
    public Seed graine;
    public ParticleSystem plantingParticles;
    private float timer;

    [Header("Effet de Gigotement")]
    [Tooltip("L'angle maximum de l'oscillation en degrés.")]
    public float jiggleAmount = 5f;
    [Tooltip("La vitesse de l'oscillation.")]
    public float jiggleSpeed = 10f;

    [Header("Effet de Récolte")]
    [Tooltip("La vitesse de rotation de la plante lors de la récolte (en degrés par seconde).")]
    public float harvestSpinSpeed = 360f;

    private Quaternion originalRotation;

    void Update()
    {
        if (isHarvesting) return;

        if (planted)
        {
            timer += Time.deltaTime;

            if (timer / graine.GrowthTimeTotal >= 1)
            {
                if (!finished)
                {
                    plante.transform.localPosition = new Vector3(0, 0.45f, 0);
                    plante.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
                    finished = true;
                    originalRotation = plante.transform.localRotation;
                }
            }
            else if (timer / graine.GrowthTimeTotal >= 0.5)
            {
                plante.transform.localPosition = new Vector3(0, 0.4f, 0);
                plante.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            }
        }

        if (finished && !isHarvesting)
        {
            float angle = jiggleAmount * Mathf.Sin(Time.time * jiggleSpeed);
            plante.transform.localRotation = originalRotation * Quaternion.Euler(0, 0, angle);
        }

        if (detected && Input.GetButtonDown("Fire1"))
        {
            HandleAction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Detecteur"))
        {
            detected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Detecteur"))
        {
            detected = false;
        }
    }

    private void HandleAction()
    {
        if (finished && !isHarvesting)
        {
            StartCoroutine(HarvestAndShrink());
        }
        else if (!planted)
        {
            plante.transform.localPosition = new Vector3(0, 0.35f, 0);
            plante.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            plante.SetActive(true);
            timer = 0;
            planted = true;
            finished = false;
            originalRotation = plante.transform.localRotation;

            // --- PARTIE MODIFIÉE ---
            if (plantingParticles != null)
            {
                Quaternion particleRotation = Quaternion.Euler(-90, 0, 0);
                Vector3 particlePosition = plante.transform.position + new Vector3(0, 1f, 0);

                // 1. On instancie les particules et on garde une référence à leur GameObject
                GameObject particleInstance = Instantiate(plantingParticles.gameObject, particlePosition, particleRotation);

                // 2. On récupère la durée totale de l'effet
                // (durée de l'émission + durée de vie maximale d'une particule)
                float totalDuration = plantingParticles.main.duration + plantingParticles.main.startLifetime.constantMax;

                // 3. On programme la destruction du GameObject après cette durée
                Destroy(particleInstance, totalDuration);
            }
        }
    }

    private IEnumerator HarvestAndShrink()
    {
        isHarvesting = true;

        float duration = 0.5f;
        Vector3 initialScale = plante.transform.localScale;
        float elapsedTime = 0f;

        plante.transform.localRotation = originalRotation;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            plante.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, progress);
            plante.transform.Rotate(Vector3.up, harvestSpinSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Mangé !");
        plante.SetActive(false);
        plante.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        plante.transform.localRotation = originalRotation;

        finished = false;
        planted = false;
        timer = 0;
        isHarvesting = false;
    }
}