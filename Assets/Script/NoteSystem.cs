using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public enum musicalNotes
{
    Do, Ré, Mi, Fa, Sol, La, Si, Do2, None
}

// Option pour choisir la direction des particules
public enum ParticleDirectionType
{
    UseJoystick,  // Utilise les inputs manette
    ObjectForward, // Tire tout droit devant l'objet
    ObjectUp,      // Tire vers le haut de l'objet
    ObjectBackward // Tire vers l'arrière de l'objet (NOUVEAU)
}

public class NoteSystem : MonoBehaviour
{
    float playedTime;
    musicalNotes noteBefore;
    float singDelay;
    bool isPlaying;
    public StudioEventEmitter music;

    [Header("Éléments d'UI")]
    [Tooltip("Glissez ici les boutons de notes de l'UI, dans le même ordre que musicalNotes.")]
    public Note[] noteUIElements;

    [Header("Effets de Particules")]
    [Tooltip("Le prefab du système de particules à instancier pour chaque note.")]
    public GameObject noteParticlePrefab;
    
    [Tooltip("Les matériaux pour chaque note, dans le même ordre que musicalNotes.")]
    public Material[] noteParticleMaterials;

    [Header("Configuration du Spawn")]
    [Tooltip("Premier point d'apparition (ex: Main Gauche).")]
    public GameObject spawnPoint1; 

    [Tooltip("Deuxième point d'apparition (ex: Main Droite).")]
    public GameObject spawnPoint2;

    [Tooltip("Comment les particules doivent-elles être orientées ?")]
    public ParticleDirectionType particleDirection = ParticleDirectionType.UseJoystick;

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
    
    public List<musicalNotes> chantDuDiab = new List<musicalNotes> { musicalNotes.Do ,musicalNotes.Ré, musicalNotes.Mi  };
    public List<musicalNotes> chantDuBonheur = new List<musicalNotes> { musicalNotes.Do, musicalNotes.Ré, musicalNotes.Do, musicalNotes.Ré };
    public List<musicalNotes> chantDuBirthday = new List<musicalNotes> { musicalNotes.Do, musicalNotes.Do, musicalNotes.Ré, musicalNotes.Do, musicalNotes.Fa, musicalNotes.Mi };
    public List<musicalNotes> playedPartition;


    private void Start()
    {
        ToggleTrackOne(true);
    }

    void Update()
    {
        playedTime += Time.deltaTime;
        PlayMusic();

        if (playedTime >= 3 && playedPartition.Count != 0)
        {
            if (!playedPartition.SequenceEqual(chantDuDiab) && !playedPartition.SequenceEqual(chantDuBonheur) && !playedPartition.SequenceEqual(chantDuBirthday))
            {
                No.Play();
            }
            playedPartition.Clear();
        }
        else if (playedPartition.Count > 0 && (playedPartition.SequenceEqual(chantDuDiab) || playedPartition.SequenceEqual(chantDuBonheur) || playedPartition.SequenceEqual(chantDuBirthday)))
        {
            noteBefore = playedPartition.LastOrDefault();
            playedPartition.Clear();
        }
    }
    
    bool IsTrackOneToggle;

    public void ToggleTrackOne(bool active)
    {
        if (!IsTrackOneToggle)
        {
            if (music != null)
            {
                if (music.IsPlaying())
                {
                    StartCoroutine(FadeSound(0.5f, active));
                    IsTrackOneToggle = true;
                }
            }
        }
    }

    IEnumerator FadeSound(float time, bool FadeIn)
    {
        int step = 5;
        float volume = FadeIn ? 1f : 0.5f;
        float ratio = 1 / step;
        
        for (int i = 0; i < step; i++)
        {
            if (!FadeIn)
            {
                volume -= ratio;
                FMOD.RESULT result = music.EventInstance.setParameterByName("Piste1_Volume", volume);
            }
            else 
            {
                volume += ratio;
                FMOD.RESULT result = music.EventInstance.setParameterByName("Piste1_Volume", volume); 
            }
            yield return new WaitForSeconds(time / 4);
        }
        if (!FadeIn)
        {
            FMOD.RESULT result = music.EventInstance.setParameterByName("Piste1_Volume", 0);
        }

        IsTrackOneToggle = false;
    }

    public void PlayNoteFromPC(int ForceNote)
    {
        isPlaying = false;
        singDelay = 0;
        StopChant();
        noteBefore = musicalNotes.None;
        PlayMusic(ForceNote);
    }

    void PlayMusic(int? ForceNote = null)
    {
        float inputX = Input.GetAxis("SongX_Xbox");
        float inputY = Input.GetAxis("SongY_Xbox");

        if (Mathf.Abs(inputX) > 0.5f || Mathf.Abs(inputY) > 0.5f)
        {
            ToggleTrackOne(false);
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
        else if(ForceNote != null)
        {
            Debug.Log("ForceNote");
            ToggleTrackOne(false);
            singDelay += Time.deltaTime;
            int noteIndex = ForceNote.Value;
            PlayNote(noteIndex);
        }
        else if(!Input.GetButton("SongPC"))
        {
            isPlaying = false;
            singDelay = 0;
            StopChant();
            noteBefore = musicalNotes.None;
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

    public musicalNotes GetNoteFromIndex(int index)
    {
        switch (index)
        {
            case 0: return musicalNotes.Do;
            case 1: return musicalNotes.Ré;
            case 2: return musicalNotes.Mi;
            case 3: return musicalNotes.Fa;
            case 4: return musicalNotes.Sol;
            case 5: return musicalNotes.La;
            case 6: return musicalNotes.Si;
            case 7: return musicalNotes.Do2;
            
            default: return musicalNotes.None;
        }
    }

    // Fonction utilitaire pour instancier une particule
    void SpawnParticleAt(GameObject spawnObj, int index)
    {
        if (spawnObj == null) return;

        Vector3 spawnPosition = spawnObj.transform.position;
        Quaternion spawnRotation = Quaternion.identity;

        // Calcul de la rotation selon le mode choisi
        switch (particleDirection)
        {
            case ParticleDirectionType.ObjectForward:
                spawnRotation = spawnObj.transform.rotation; // Suit la rotation de l'objet
                break;

            case ParticleDirectionType.ObjectBackward:
                // Rotation à 180 degrés autour de l'axe Y pour faire face à l'arrière
                spawnRotation = spawnObj.transform.rotation * Quaternion.Euler(0, 180, 0);
                break;
            
            case ParticleDirectionType.ObjectUp:
                // Regarde vers le haut de l'objet (90 degrés sur l'axe X local par rapport à forward)
                spawnRotation = spawnObj.transform.rotation * Quaternion.Euler(-90, 0, 0); 
                break;

            case ParticleDirectionType.UseJoystick:
            default:
                // Ancien comportement : Joystick
                Vector3 direction = new Vector3(Input.GetAxis("SongX_Xbox"), 0f, Input.GetAxis("SongY_Xbox"));
                if (direction.sqrMagnitude > 0.01f)
                {
                    spawnRotation = Quaternion.LookRotation(direction);
                }
                break;
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

    void PlayNote(int index)
    {
        if (GetNoteFromIndex(index) != noteBefore || playedTime > 3)
        {
            if (noteUIElements != null && index >= 0 && index < noteUIElements.Length && noteUIElements[index] != null)
            {
                noteUIElements[index].Highlight();
            }

            if (!isPlaying)
            {
                StartChant(index);
            }
            
            if (singDelay != 0.00f)
            {
                isPlaying = false;
                playedPartition.Add(GetNoteFromIndex(index));
                noteBefore = GetNoteFromIndex(index);

                if (noteParticlePrefab != null && noteParticleMaterials != null && index < noteParticleMaterials.Length && noteParticleMaterials[index] != null)
                {
                    // --- MODIFICATION ICI ---
                    // On spawn aux deux endroits si possible
                    if (spawnPoint1 != null)
                    {
                        SpawnParticleAt(spawnPoint1, index);
                    }
                    else
                    {
                         // Fallback si rien n'est assigné : spawn sur le joueur lui-même
                         SpawnParticleAt(this.gameObject, index);
                    }

                    if (spawnPoint2 != null)
                    {
                        SpawnParticleAt(spawnPoint2, index);
                    }
                    // ------------------------
                }

                if (playedPartition.TakeLast(3).SequenceEqual(chantDuDiab)) { StartCoroutine(VictoryPlay()); Debug.LogWarning("Chant du diabète"); StartCoroutine(GameManager.Instance.playerManager.SetSingingStateCalme(15)); }
                else if (playedPartition.TakeLast(4).SequenceEqual(chantDuBonheur)) { StartCoroutine(VictoryPlay()); Debug.LogWarning("Chant du Bonheur");}
                else if (playedPartition.SequenceEqual(chantDuBirthday)) { Debug.LogWarning("Chant de l'anniversaire"); }
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

    public bool PlayerSingCorrectPattern(List<musicalNotes> Pattern)
    {
        if (playedPartition.TakeLast(Pattern.Count).SequenceEqual(Pattern))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}