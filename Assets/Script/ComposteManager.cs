using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComposteManager : MonoBehaviour
{
    private bool detected = false;
    public ParticleSystem dirtParticleSystem; // Assign your particle system in the Inspector
    public float particleSystemExtraLifetime = 2.0f; // Extra time in seconds before destroying the particle system
    public float particleYOffset = 1.0f; // Hauteur de spawn des particules
    public Vector3 particleRotation = new Vector3(90, 0, 0); // Rotation des particules

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 2 && GameManager.Instance.playerManager.terre < GameManager.Instance.playerManager.terreMax)
        {
            if (GameManager.Instance.playerManager.indexTuto == 1)
            {
                GameManager.Instance.playerManager.indexTuto += 1;
                GameManager.Instance.playerManager.tutoSelect = GameManager.Instance.playerManager.tuto[GameManager.Instance.playerManager.indexTuto];
            }

            GameManager.Instance.playerManager.terre = GameManager.Instance.playerManager.terreMax;

            // Create and play the particle system if it's assigned
            if (dirtParticleSystem != null)
            {
                // Définit la position et la rotation pour l'instanciation
                Vector3 spawnPosition = transform.position + Vector3.up * particleYOffset;
                Quaternion spawnRotation = Quaternion.Euler(particleRotation);

                // Instancie le système de particules avec la position et la rotation spécifiées
                ParticleSystem ps = Instantiate(dirtParticleSystem, spawnPosition, spawnRotation);

                // Détruit l'objet du système de particules après sa durée principale plus la durée de vie supplémentaire
                Destroy(ps.gameObject, ps.main.duration + particleSystemExtraLifetime);
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