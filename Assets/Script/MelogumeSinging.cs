using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MelogumeSingingManager : MonoBehaviour
{
    [Header("Événements FMOD")]
    [Tooltip("Assignez ici l'émetteur FMOD pour la note DO.")]
    public FMODUnity.StudioEventEmitter DO;
    [Tooltip("Assignez ici l'émetteur FMOD pour la note RE.")]
    public FMODUnity.StudioEventEmitter RE;
    [Tooltip("Assignez ici l'émetteur FMOD pour la note MI.")]
    public FMODUnity.StudioEventEmitter MI;

    [Header("Effets de Particules")]
    [Tooltip("Le prefab du système de particules à instancier pour chaque note.")]
    public GameObject noteParticlePrefab;
    [Tooltip("Le point d'apparition des particules. Si non défini, la position de cet objet sera utilisée.")]
    public Transform particleSpawnPoint;
    [Tooltip("Le matériau pour les particules de la note DO.")]
    public Material doMaterial;
    [Tooltip("Le matériau pour les particules de la note RE.")]
    public Material reMaterial;
    [Tooltip("Le matériau pour les particules de la note MI.")]
    public Material miMaterial;

    public Coroutine joyeux;
    public Coroutine rage;

    private bool _isGameManagerReady = false;
    [SerializeField] LegumeManager legumeManager;

    void Start()
    {
        legumeManager = GetComponent<LegumeManager>();

        if (DO == null || RE == null || MI == null)
        {
            Debug.LogError("Attention : Les émetteurs FMOD (DO, RE, MI) ne sont pas assignés dans l'Inspecteur du GameObject " + gameObject.name + ". La chanson ne démarrera pas.");
            return;
        }

        if (GameManager.Instance != null && legumeManager != null)
        {
            _isGameManagerReady = true;
        }

        joyeux = StartCoroutine(SongOfHealing());
    }

    // Arrête tous les sons joués par ce script
    void StopChant()
    {
        if (DO != null) DO.Stop();
        if (RE != null) RE.Stop();
        if (MI != null) MI.Stop();
    }

    /// <summary>
    /// Joue une note et déclenche l'effet de particules associé.
    /// </summary>
    /// <param name="noteEmitter">L'émetteur FMOD de la note à jouer.</param>
    /// <param name="particleMaterial">Le matériau à appliquer aux particules.</param>
    void PlayNoteWithParticles(FMODUnity.StudioEventEmitter noteEmitter, Material particleMaterial)
    {
        // 1. Jouer le son
        noteEmitter.Play();

        // 2. Créer les particules si tout est configuré
        if (noteParticlePrefab != null && particleMaterial != null)
        {
            // Détermine la position et la rotation
            Vector3 spawnPosition = particleSpawnPoint != null ? particleSpawnPoint.position : transform.position;
            // Les particules sont orientées dans la même direction que le GameObject
            Quaternion spawnRotation = transform.rotation;

            GameObject particleInstance = Instantiate(noteParticlePrefab, spawnPosition, spawnRotation);
            ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                // Applique le bon matériau
                var renderer = ps.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.material = particleMaterial;
                }
                // Détruit l'objet après la fin de l'effet
                Destroy(particleInstance, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(particleInstance, 5f); // Sécurité
            }
        }
    }

    public IEnumerator SongOfHealing()
    {
        // Gérer la vitesse uniquement si la référence GameManager est prête
        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 0;

        }

        // --- Séquence musicale avec particules ---

        PlayNoteWithParticles(DO, doMaterial);
        yield return new WaitForSeconds(1);
        StopChant();

        PlayNoteWithParticles(RE, reMaterial);
        yield return new WaitForSeconds(1);
        StopChant();

        PlayNoteWithParticles(DO, doMaterial);
        yield return new WaitForSeconds(1);
        StopChant();

        PlayNoteWithParticles(MI, miMaterial);
        yield return new WaitForSeconds(1);
        StopChant();

        // Rétablir la vitesse de déplacement
        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 5;
        }

        // Attente aléatoire avant de répéter la chanson
        yield return new WaitForSeconds(Random.Range(3f, 10.0f));

        // Répéter la chanson
        joyeux = StartCoroutine(SongOfHealing());
        
    }
    public void StopHappyness()
    {
        StopCoroutine(joyeux);
    }

    public void StartHappyness()
    {
        joyeux = StartCoroutine(SongOfHealing());
    }
    public IEnumerator SongOfRage()
    {
        // Gérer la vitesse uniquement si la référence GameManager est prête
        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 0;
        }
        
        
        // --- Séquence musicale avec particules ---

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



        // Rétablir la vitesse de déplacement
        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 5;
        }

        // Attente aléatoire avant de répéter la chanson
        yield return new WaitForSeconds(Random.Range(3f, 10.0f));

        // Répéter la chanson
        rage = StartCoroutine(SongOfRage());
    }

    public void StartRage()
    {
        rage = StartCoroutine(SongOfRage());
    }
    public void StopRage()
    {
        StopCoroutine(rage);
    }
}
