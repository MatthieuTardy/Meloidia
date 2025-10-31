using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMODUnity;

public class NoteSystem : MonoBehaviour
{

    float playedTime;
    string noteBefore;
    float singDelay;
    bool isPlaying;

    [Header("Éléments d'UI")]
    [Tooltip("Glissez ici les boutons de notes de l'UI, dans le même ordre que musicalNotes.")]
    public Note[] noteUIElements;

    [Header("Effets de Particules")]
    [Tooltip("Le prefab du système de particules à instancier pour chaque note.")]
    public GameObject noteParticlePrefab;
    [Tooltip("Les matériaux pour chaque note, dans le même ordre que musicalNotes.")]
    public Material[] noteParticleMaterials;
    [Tooltip("Le point d'apparition pour les particules. Si non défini, la position de cet objet sera utilisée.")]
    public Transform particleSpawnPoint;

    [Header("Événements FMOD")]
    public FMODUnity.StudioEventEmitter DO;
    public FMODUnity.StudioEventEmitter RE;
    public FMODUnity.StudioEventEmitter MI;
    public FMODUnity.StudioEventEmitter FA;
    public FMODUnity.StudioEventEmitter SOL;
    public FMODUnity.StudioEventEmitter LA;
    public FMODUnity.StudioEventEmitter SI;
    public FMODUnity.StudioEventEmitter DO2;

    public FMODUnity.StudioEventEmitter Victoire;
    public FMODUnity.StudioEventEmitter No;

    public int essenceGainOnMelody = 75;

    public string[] musicalNotes = new string[8]
    {
        "Do (Nord)", "Ré (Nord-Est)", "Mi (Est)", "Fa (Sud-Est)",
        "Sol (Sud)", "La (Sud-Ouest)", "Si (Ouest)", "Do' (Nord-Ouest)"
    };
    public List<string> listeDeNotes = new List<string> { "Do (Nord)", "Ré (Nord-Est)", "Mi (Est)" };
    public List<string> listeDeNotes2 = new List<string> { "Do (Nord)", "Ré (Nord-Est)", "Do (Nord)", "Mi (Est)" };
    public List<string> listeDeNotes3 = new List<string> { "Do (Nord)", "Do (Nord)", "Ré (Nord-Est)", "Do (Nord)", "Fa (Sud-Est)", "Mi (Est)" };
    public List<string> playedPartition;

    void Update()
    {
        playedTime += Time.deltaTime;
        PlayMusic();

        if (playedTime >= 3 && playedPartition.Count != 0)
        {
            if (!playedPartition.SequenceEqual(listeDeNotes) && !playedPartition.SequenceEqual(listeDeNotes2) && !playedPartition.SequenceEqual(listeDeNotes3))
            {
                No.Play();
            }
            playedPartition.Clear();
        }
        else if (playedPartition.Count > 0 && (playedPartition.SequenceEqual(listeDeNotes) || playedPartition.SequenceEqual(listeDeNotes2) || playedPartition.SequenceEqual(listeDeNotes3)))
        {
            noteBefore = playedPartition.LastOrDefault();
            playedPartition.Clear();
        }
    }

    private void PlayMusic()
    {
        float inputX = Input.GetAxis("SongX_Xbox");
        float inputY = Input.GetAxis("SongY_Xbox");

        if (Mathf.Abs(inputX) > 0.5f || Mathf.Abs(inputY) > 0.5f)
        {
            singDelay += Time.deltaTime;
            int noteIndex = 0;

            if (inputY > 0.5f && Mathf.Abs(inputX) < 0.5f) { noteIndex = 4; }
            else if (inputX > 0.3f && inputY > 0.3f) { noteIndex = 3; }
            else if (inputX > 0.5f && Mathf.Abs(inputY) < 0.5f) { noteIndex = 2; }
            else if (inputX > 0.3f && inputY < -0.3f) { noteIndex = 1; }
            else if (inputY < -0.5f && Mathf.Abs(inputX) < 0.5f) { noteIndex = 0; }
            else if (inputX < -0.3f && inputY < -0.3f) { noteIndex = 7; }
            else if (inputX < -0.5f && Mathf.Abs(inputY) < 0.5f) { noteIndex = 6; }
            else if (inputX < -0.3f && inputY > 0.3f) { noteIndex = 5; }

            PlayNote(noteIndex);
        }
        else
        {
            isPlaying = false;
            singDelay = 0;
            StopChant();
            noteBefore = null;
        }
    }

    void StopChant()
    {
        DO.Stop(); RE.Stop(); MI.Stop(); FA.Stop();
        SOL.Stop(); LA.Stop(); SI.Stop(); DO2.Stop();
    }

    void StartChant(int index)
    {
        StopChant();
        switch (index)
        {
            case 0: DO.Play(); break;
            case 1: RE.Play(); break;
            case 2: MI.Play(); break;
            case 3: FA.Play(); break;
            case 4: SOL.Play(); break;
            case 5: LA.Play(); break;
            case 6: SI.Play(); break;
            case 7: DO2.Play(); break;
        }
    }

    void PlayNote(int index)
    {
        if (musicalNotes[index] != noteBefore || playedTime > 3)
        {
            if (noteUIElements != null && index >= 0 && index < noteUIElements.Length && noteUIElements[index] != null)
            {
                noteUIElements[index].Highlight();
            }

            if (!isPlaying)
            {
                StartChant(index);
            }

            if (singDelay >= 0.2f)
            {
                isPlaying = false;
                playedPartition.Add(musicalNotes[index]);
                noteBefore = musicalNotes[index];

                if (noteParticlePrefab != null && noteParticleMaterials != null && index < noteParticleMaterials.Length && noteParticleMaterials[index] != null)
                {
                    Vector3 spawnPosition = particleSpawnPoint != null ? particleSpawnPoint.position : transform.position;

                    // On crée un vecteur de direction 3D sur le plan XZ. L'axe Y du joystick contrôle l'axe Z du monde.
                    Vector3 direction = new Vector3(Input.GetAxis("SongX_Xbox"), 0f, Input.GetAxis("SongY_Xbox"));

                    // On s'assure que le vecteur n'est pas nul pour éviter une erreur avec LookRotation.
                    Quaternion spawnRotation = Quaternion.identity; // Rotation par défaut.
                    if (direction.sqrMagnitude > 0.01f)
                    {
                        // Crée une rotation qui "regarde" dans la direction du joystick sur le plan XZ.
                        spawnRotation = Quaternion.LookRotation(direction);
                    }

                    GameObject particleInstance = Instantiate(noteParticlePrefab, spawnPosition, spawnRotation);

                    ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        var renderer = ps.GetComponent<ParticleSystemRenderer>();
                        if (renderer != null)
                        {
                            renderer.material = noteParticleMaterials[index];
                        }
                        Destroy(particleInstance, ps.main.duration + ps.main.startLifetime.constantMax);
                    }
                    else
                    {
                        Destroy(particleInstance, 5f);
                    }
                }

                if (playedPartition.SequenceEqual(listeDeNotes)) { StartCoroutine(VictoryPlay()); Debug.LogWarning("Chant du diabète"); }
                else if (playedPartition.SequenceEqual(listeDeNotes2)) { StartCoroutine(VictoryPlay()); Debug.LogWarning("Chant du Bonheur"); GameManager.Instance.playerManager.essenceMagique += essenceGainOnMelody; }
                else if (playedPartition.SequenceEqual(listeDeNotes3)) { Debug.LogWarning("Chant de l'anniversaire"); }
            }
            else
            {
                isPlaying = true;
            }
        }
        playedTime = 0;
    }

    IEnumerator VictoryPlay()
    {
        yield return new WaitForSeconds(0.5f);
        Victoire.Play();
    }
}
