using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuitManager : MonoBehaviour
{
    private bool detected = false;

    [Header("Effet de Particules")]
    public ParticleSystem waterFillParticles; // Le prfabriqu de particules  instancier
    public Transform particleSpawnPoint;      // L'objet vide qui dfinit la position et la rotation du spawn
    public float particleSystemExtraLifetime = 2.0f; // Temps supplmentaire avant de dtruire les particules
    
    // Reference au PlayerController
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 1)
        {
            if (GameManager.Instance.playerManager.indexTuto == 3)
            {
                GameManager.Instance.playerManager.indexTuto += 1;
                GameManager.Instance.playerManager.tutoSelect = GameManager.Instance.playerManager.tuto[GameManager.Instance.playerManager.indexTuto];
            }
            GameManager.Instance.playerManager.ReloadWater();

            // Joue l'effet de particules si tout est assign
            if (waterFillParticles != null && particleSpawnPoint != null)
            {
                // Instancie le systme de particules  la position et rotation du point de spawn
                ParticleSystem ps = Instantiate(waterFillParticles, particleSpawnPoint.position, particleSpawnPoint.rotation);

                // Calcule la dure totale de l'effet
                float totalDuration = ps.main.duration + ps.main.startLifetime.constantMax + particleSystemExtraLifetime;

                // Dtruit l'objet de particules aprs sa dure de vie complte
                Destroy(ps.gameObject, totalDuration);
            }
            
            // LANCE L'ANIMATION
            if (playerController != null)
            {
                playerController.TriggerActionAnimation();
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