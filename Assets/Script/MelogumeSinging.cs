using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MelogumeSingingManager : MonoBehaviour
{
    [Header("�v�nements FMOD")]
    [Tooltip("Assignez ici l'�metteur FMOD pour la note DO.")]
    public FMODUnity.StudioEventEmitter DO;
    [Tooltip("Assignez ici l'�metteur FMOD pour la note RE.")]
    public FMODUnity.StudioEventEmitter RE;
    [Tooltip("Assignez ici l'�metteur FMOD pour la note MI.")]
    public FMODUnity.StudioEventEmitter MI;

    [Header("Effets de Particules")]
    [Tooltip("Le prefab du syst�me de particules � instancier pour chaque note.")]
    public GameObject noteParticlePrefab;
    [Tooltip("Le point d'apparition des particules. Si non d�fini, la position de cet objet sera utilis�e.")]
    public Transform particleSpawnPoint;
    [Tooltip("Le mat�riau pour les particules de la note DO.")]
    public Material doMaterial;
    [Tooltip("Le mat�riau pour les particules de la note RE.")]
    public Material reMaterial;
    [Tooltip("Le mat�riau pour les particules de la note MI.")]
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
            Debug.LogError("Attention : Les �metteurs FMOD (DO, RE, MI) ne sont pas assign�s dans l'Inspecteur du GameObject " + gameObject.name + ". La chanson ne d�marrera pas.");
            return;
        }

        if (GameManager.Instance != null && legumeManager != null)
        {
            _isGameManagerReady = true;
        }

        joyeux = StartCoroutine(SongOfHealing());
    }

    // Arr�te tous les sons jou�s par ce script
    void StopChant()
    {
        if (DO != null) DO.Stop();
        if (RE != null) RE.Stop();
        if (MI != null) MI.Stop();
    }

    /// <summary>
    /// Joue une note et d�clenche l'effet de particules associ�.
    /// </summary>
    /// <param name="noteEmitter">L'�metteur FMOD de la note � jouer.</param>
    /// <param name="particleMaterial">Le mat�riau � appliquer aux particules.</param>
    void PlayNoteWithParticles(FMODUnity.StudioEventEmitter noteEmitter, Material particleMaterial)
    {
        // 1. Jouer le son
        noteEmitter.Play();

        // 2. Cr�er les particules si tout est configur�
        if (noteParticlePrefab != null && particleMaterial != null)
        {

            // D�termine la position et la rotation
            Vector3 spawnPosition = particleSpawnPoint != null ? particleSpawnPoint.position : transform.position;
            // Les particules sont orient�es dans la m�me direction que le GameObject
            Quaternion spawnRotation = transform.rotation;

            GameObject particleInstance = Instantiate(noteParticlePrefab, spawnPosition, spawnRotation);
            ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                // Applique le bon mat�riau
                var renderer = ps.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.material = particleMaterial;
                }
                // D�truit l'objet apr�s la fin de l'effet
                Destroy(particleInstance, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(particleInstance, 5f); // S�curit�
            }
        }
    }

    public IEnumerator SongOfHealing()
    {
        // G�rer la vitesse uniquement si la r�f�rence GameManager est pr�te
        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 0;

        }

        // --- S�quence musicale avec particules ---
        legumeManager.animator.SetBool("walk", false);
        legumeManager.animator.SetBool("sing", true);

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

        legumeManager.animator.SetBool("sing", false);
        // R�tablir la vitesse de d�placement

        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 5;
        }

        // Attente al�atoire avant de r�p�ter la chanson
        yield return new WaitForSeconds(Random.Range(3f, 10.0f));

        // R�p�ter la chanson
        joyeux = StartCoroutine(SongOfHealing());
        
    }
    public void StopHappyness()
    {
        StopCoroutine(joyeux);
        legumeManager.animator.SetBool("sing", false);
    }

    public void StartHappyness()
    {
        joyeux = StartCoroutine(SongOfHealing());
    }
    public IEnumerator SongOfRage()
    {
        // G�rer la vitesse uniquement si la r�f�rence GameManager est pr�te
        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 0;
        }


        // --- S�quence musicale avec particules ---
        legumeManager.animator.SetBool("walk", false);
        legumeManager.animator.SetBool("sing", true);
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
        legumeManager.animator.SetBool("sing", false);


        // R�tablir la vitesse de d�placement
        if (_isGameManagerReady)
        {
            legumeManager.vitesse = 5;
        }

        // Attente al�atoire avant de r�p�ter la chanson
        yield return new WaitForSeconds(Random.Range(3f, 10.0f));

        // R�p�ter la chanson
        rage = StartCoroutine(SongOfRage());
    }

    public void StartRage()
    {
        legumeManager.animator.SetBool("sing", false);
        rage = StartCoroutine(SongOfRage());
    }
    public void StopRage()
    {
        StopCoroutine(rage);
    }
}
