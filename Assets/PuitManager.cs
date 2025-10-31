using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuitManager : MonoBehaviour
{
    private bool detected = false;

    [Header("Effet de Particules")]
    public ParticleSystem waterFillParticles; // Le préfabriqué de particules à instancier
    public Transform particleSpawnPoint;      // L'objet vide qui définit la position et la rotation du spawn
    public float particleSystemExtraLifetime = 2.0f; // Temps supplémentaire avant de détruire les particules

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 1 && GameManager.Instance.playerManager.eau < GameManager.Instance.playerManager.eauMax)
        {
            if (GameManager.Instance.playerManager.indexTuto == 3)
            {
                GameManager.Instance.playerManager.indexTuto += 1;
                GameManager.Instance.playerManager.tutoSelect = GameManager.Instance.playerManager.tuto[GameManager.Instance.playerManager.indexTuto];
            }
            GameManager.Instance.playerManager.eau = GameManager.Instance.playerManager.eauMax;

            // Joue l'effet de particules si tout est assigné
            if (waterFillParticles != null && particleSpawnPoint != null)
            {
                // Instancie le système de particules à la position et rotation du point de spawn
                ParticleSystem ps = Instantiate(waterFillParticles, particleSpawnPoint.position, particleSpawnPoint.rotation);

                // Calcule la durée totale de l'effet
                float totalDuration = ps.main.duration + ps.main.startLifetime.constantMax + particleSystemExtraLifetime;

                // Détruit l'objet de particules après sa durée de vie complète
                Destroy(ps.gameObject, totalDuration);
            }
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
}