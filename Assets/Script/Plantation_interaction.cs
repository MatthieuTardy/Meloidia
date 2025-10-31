using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Plantation_interaction : MonoBehaviour
{

    [Header("Init")]
    private bool detected = false;

    [Header("State")]
    private bool watered = false;
    private bool prepared = false;
    private bool clean = false;

    private bool planted = false;
    private bool finished = false;
    private bool isHarvesting = false;
    private bool isPlanting = false; // Pour gérer l'état de l'animation de plantation
    public int happyness = 0;

    public GameObject legume;

    public GameObject plante;
    public Seed graine;

    public ParticleSystem plantingParticles; // Renommé, car c'est seulement pour la plantation
    private float timer;

    [Header("Effet de Gigotement")]
    public float jiggleAmount = 5f;
    public float jiggleSpeed = 10f;

    [Header("Effet de Récolte")]
    public float harvestSpinSpeed = 360f;
    public int essenceGainOnHarvest = 25;

    private Quaternion originalRotation;

    private void Start()
    {

    }

    void Update()
    {
        // On bloque les updates si une animation est en cours
        if (isHarvesting || isPlanting) return;

        if (planted)
        {
            timer += Time.deltaTime;

            if (timer / graine.GrowthTimeTotal >= 1)
            {
                if (!finished)
                {
                    plante.transform.localPosition = new Vector3(0, 0.30f, 0);
                    plante.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
                    finished = true;
                    originalRotation = plante.transform.localRotation;
                }
            }
            else if (timer / graine.GrowthTimeTotal >= 0.5)
            {
                plante.transform.localPosition = new Vector3(0, 0.25f, 0);
                plante.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            }
        }

        if (finished)
        {
            float angle = jiggleAmount * Mathf.Sin(Time.time * jiggleSpeed);
            plante.transform.localRotation = originalRotation * Quaternion.Euler(0, 0, angle);
        }

        if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 0 && prepared == true && clean == true)
        {
            // --- DÉBUT DE LA CORRECTION ---
            PlayerManager playerManager = GameManager.Instance.playerManager;

            // On vérifie si l'index suivant est valide avant de l'incrémenter
            if (playerManager.indexTuto == 2 && (playerManager.indexTuto + 1) < playerManager.tuto.Count)
            {
                playerManager.indexTuto++;
                playerManager.tutoSelect = playerManager.tuto[playerManager.indexTuto];
            }
            // On vérifie si l'index suivant est valide avant de l'incrémenter
            else if (playerManager.indexTuto == 4 && (playerManager.indexTuto + 1) < playerManager.tuto.Count)
            {
                playerManager.indexTuto++;
                playerManager.tutoSelect = playerManager.tuto[playerManager.indexTuto];
            }
            // --- FIN DE LA CORRECTION ---

            HandleAction();
        }
        else if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 1 && watered == false && GameManager.Instance.playerManager.eau > 0)
        {
            GameManager.Instance.playerManager.eau -= 1;
            watered = true;
            timer += 10;
            happyness += 10;
            Debug.Log("Arrosé");
        }
        else if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 2 && prepared == false && GameManager.Instance.playerManager.terre > 0)
        {
            GameManager.Instance.playerManager.terre -= 1;
            prepared = true;
            Debug.Log("Terre est mise");
        }
        else if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 3 && clean == false && prepared == true)
        {
            clean = true;

            Debug.Log("Terrain propre");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Detecteur")) { detected = true; }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Detecteur")) { detected = false; }
    }

    private void HandleAction()
    {
        // S'assure qu'aucune autre action n'est en cours
        if (isHarvesting || isPlanting) return;

        if (finished)
        {
            StartCoroutine(HarvestAndShrink());
        }
        else if (!planted)
        {
            StartCoroutine(PlantAndGrow());
        }
    }

    // NOUVELLE Coroutine pour l'animation de plantation
    private IEnumerator PlantAndGrow()
    {
        isPlanting = true;

        // Joue les particules au début de la plantation
        PlayPlantingParticles();

        float duration = 0.3f; // Animation rapide
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);
        float elapsedTime = 0f;

        // Prépare la plante mais la garde invisible (scale = 0)
        plante.transform.localPosition = new Vector3(0, 0.20f, 0);
        plante.transform.localScale = initialScale;
        plante.SetActive(true);
        originalRotation = plante.transform.localRotation; // Sauvegarder la rotation de base

        // Boucle d'animation
        while (elapsedTime < duration)
        {
            plante.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // S'assure que la taille est correcte à la fin
        plante.transform.localScale = targetScale;

        // Finalise l'état de plantation
        planted = true;
        finished = false;
        timer = 0;
        isPlanting = false; // L'animation est terminée
    }

    private IEnumerator HarvestAndShrink()
    {
        isHarvesting = true;

        GameManager.Instance.playerManager.essenceMagique += essenceGainOnHarvest;

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

        Instantiate(legume, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z - 3), new Quaternion(0, 180, 0, 0));
        Debug.Log("Récolté ! Essence gagnée : " + essenceGainOnHarvest);
        clean = false;
        prepared = false;
        watered = false;
        plante.SetActive(false);
        plante.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        plante.transform.localRotation = originalRotation;

        finished = false;
        planted = false;
        timer = 0;
        isHarvesting = false;
    }



    private void PlayPlantingParticles()
    {
        if (plantingParticles != null)
        {
            Quaternion particleRotation = Quaternion.Euler(-90, 0, 0);
            Vector3 particlePosition = transform.position + new Vector3(0, 1f, 0);

            GameObject particleInstance = Instantiate(plantingParticles.gameObject, particlePosition, particleRotation);

            // Assurez-vous que le ParticleSystem est en lecture
            ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();
            if (ps != null && !ps.isPlaying)
            {
                ps.Play();
            }

            float totalDuration = ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : 1f; // Vérification de nullité
            Destroy(particleInstance, totalDuration);
        }
    }
}