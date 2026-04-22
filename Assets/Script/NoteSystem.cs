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
    UseJoystick, 
    ObjectForward, 
    ObjectUp,     
    ObjectBackward 
}

public class NoteSystem : MonoBehaviour
{
    float playedTime;
    musicalNotes noteBefore;
    musicalNotes noteCurrent;
    public musicalNotes noteHolded;
    [SerializeField] float HoldDelay;
    [SerializeField] float ClearDelay = 2f;



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

    bool toggleTrackBool;
    private void Start()
    {
        toggleTrackBool = true;
        ToggleTrackOne(true);
        WheelCenter = FindAnyObjectByType<UiSelection>().wheelRoot.transform;
    }

    void Update()
    {

        playedTime += Time.deltaTime;

        float value = Input.GetAxisRaw("ToggleSing");
        value += Input.GetAxisRaw("SongPC");
        if (value > 0 && !IsToggleSing)
        {
            isSinging = !isSinging;
            IsToggleSing = true;
        }

        if (value < 0.1f)
        {
            IsToggleSing = false;
        }


        if (isSinging)
        {
            PlayMusic();
        }

        //Debug.Log("PlayedTime : " + singDelay);
        if ((playedTime >= HoldDelay || singDelay >= HoldDelay) && Input.GetAxisRaw("ValidateNote") > 0)
        {
            addHoldedNote();
        }
        else if (Input.GetAxis("ValidateNote") < 0)
        {
            //StopChant();
        }



        if (playedTime >= ClearDelay && playedPartition.Count != 0)
        {
            playedPartition.Clear();
            noteCurrent = musicalNotes.None;
            clearHoldedNote();
        }

        if (noteHolded != musicalNotes.None)
        {

            if (noteCurrent != noteHolded)
            {
                clearHoldedNote();
                playedTime = 0;
                singDelay = 0;
            }
        }

        if (Input.GetButtonDown("SongPC"))
        {
            toggleTrackBool = !toggleTrackBool;
            ToggleTrackOne(toggleTrackBool);
        }

        
    }
    public void ClearPartition()
    {
        playedPartition.Clear();
        noteCurrent = musicalNotes.None;
        clearHoldedNote();
    }
    bool IsTrackOneToggle;

    public void ToggleTrackSpecial()
    {
        ToggleTrackOne(toggleTrackBool);
    }

    public void ToggleTrackOne(bool active)
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
    private float volumeT1;
    private bool isSinging;

    public bool IsToggleSing;

    IEnumerator FadeSound(float time, bool FadeIn)
    {
        int step = 5;
        //A voir (Test)
        volumeT1 = FadeIn ? 1f : 0.5f;


        float ratio = 1 / step;

        for (int i = 0; i < step; i++)
        {
            if (!FadeIn)
            {
                volumeT1 -= ratio;
                FMOD.RESULT result = music.EventInstance.setParameterByName("Melodie1", volumeT1);
            }
            else 
            {
                volumeT1 += ratio;
                FMOD.RESULT result = music.EventInstance.setParameterByName("Melodie1", volumeT1); 
            }
            yield return new WaitForSeconds(time / 4);
        }
        if (!FadeIn)
        {
            FMOD.RESULT result = music.EventInstance.setParameterByName("Melodie1", 0);
        }

        IsTrackOneToggle = false;
    }

    public void PlayNoteFromPC(int ForceNote)
    {
        isPlaying = false;
        singDelay = 0;
        //StopChant();
        noteBefore = musicalNotes.None;
        PlayMusic(ForceNote);
    }

    void PlayMusic(int? ForceNote = null)
    {
        Vector2 Controller = new Vector2(Input.GetAxis("SongX_Xbox"), Input.GetAxis("SongY_Xbox"));
        Vector2 mouseDir = GetMouseDirection();

        int noteIndex = 0;
        if(Controller.x != 0 || Controller.y != 0) 
        {
            if (Mathf.Abs(Controller.x) > 0.5f || Mathf.Abs(Controller.y) > 0.5f)
            {

                if (Input.GetAxisRaw("ValidateNote") > 0)
                {
                    singDelay += Time.deltaTime;
                }
                else
                {
                    singDelay = 0;
                    clearHoldedNote();
                }

                if (Controller.y > 0.5f && Mathf.Abs(Controller.x) < 0.5f) { noteIndex = 4; }
                else if (Controller.x > 0.3f && Controller.y < -0.3f) { noteIndex = 1; }
                else if (Controller.x > 0.3f && Controller.y > 0.3f) { noteIndex = 3; }
                else if (Controller.x > 0.5f && Mathf.Abs(Controller.y) < 0.5f) { noteIndex = 2; }
                else if (Controller.y < -0.5f && Mathf.Abs(Controller.x) < 0.5f) { noteIndex = 0; }
                else if (Controller.x < -0.3f && Controller.y < -0.3f) { noteIndex = 7; }
                else if (Controller.x < -0.5f && Mathf.Abs(Controller.y) < 0.5f) { noteIndex = 6; }
                else if (Controller.x < -0.3f && Controller.y > 0.3f) { noteIndex = 5; }

                noteUIElements[noteIndex].Highlight();
                if (Input.GetAxisRaw("ValidateNote") > 0)
                {
                    Debug.Log("Valide Note " + noteIndex);
                    PlayNote(noteIndex);
                }

            }
        } // on est manette
        else 
        {
            //Debug.Log("mouseDir " + mouseDir);
            if (mouseDir != Vector2.zero)
            {
                if (Input.GetAxisRaw("ValidateNote") > 0)
                {
                    singDelay += Time.deltaTime;
                }
                else
                {
                    singDelay = 0;
                    clearHoldedNote();
                }
                float angle = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
                if (angle < 90 + 22.5 && angle > 90 - 22.5)
                {
                    //Debug.Log("DO");
                    noteIndex = 0;
                }// haut DO
                else if (angle < 45 + 22.5 && angle > 45 - 22.5)
                {
                   // Debug.Log("Ré");
                    noteIndex = 1;
                }// haut droite Ré
                else if (angle < 0 + 22.5 && angle > 0 - 22.5)
                {
                    //Debug.Log("Mi");
                    noteIndex = 2;
                } // droite Mi
                else if (angle < -45 + 22.5 && angle > -45 - 22.5)
                {
                   // Debug.Log("Fa");
                    noteIndex = 3;
                } // bas droite fa
                else if (angle < -90 + 22.5 && angle > -90 - 22.5)
                {
                   // Debug.Log("Sol");
                    noteIndex = 4;
                } // bas Sol
                else if (angle > -135 - 22.5 && angle < -135 + 22.5)
                {
                   // Debug.Log("La");
                    noteIndex = 5;
                } // bas gauche La
                else if (angle > 180 - 22.5 || angle < -180 + 22.5)
                {
                   // Debug.Log("Si");
                    noteIndex = 6;
                }//gauche Si
                else if (angle > 135 - 22.5 && angle < 135 + 22.5)
                {
                   // Debug.Log("Do2");
                    noteIndex = 7;
                } // haut gauche DO2

                noteUIElements[noteIndex].Highlight();
                if (Input.GetAxisRaw("ValidateNote") > 0)
                {
                    Debug.Log("Valide Note " + noteIndex);
                    PlayNote(noteIndex);
                }
            }

        } // on est souris

        
        
        /*

        else if(ForceNote != null)
        {
            //Debug.Log("ForceNote");
            
            if (Input.GetButton("ValidateNote"))
            {
                singDelay += Time.deltaTime;
            }
            else
            {
                singDelay = 0;
                clearHoldedNote();
            }
            int noteIndex = ForceNote.Value;
            PlayNote(noteIndex);
        }
        */
        /*
        else if(!Input.GetButton("SongPC"))
        {
            isPlaying = false;
            singDelay = 0;
            StopChant();
            noteBefore = musicalNotes.None;
        }
        */
    }
    Transform WheelCenter;
    Vector2 GetMouseDirection()
    {
        Vector2 screenCenter = WheelCenter.position;
        //Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mousePos = Input.mousePosition;

        // Offset from center (positive X = right, positive Y = up)
        Vector3 offsetFromCenter = mousePos - screenCenter;

        
        //Debug.Log("offsetFromCenter " + offsetFromCenter);
        
       // Vector2 mouseDir = screenCenter - mouseDirEnd;
        
        if(Vector2.Distance(mousePos,screenCenter) >= 100f)
        {
            return offsetFromCenter;
        }

        else
        {
            return Vector2.zero;
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
                noteCurrent = GetNoteFromIndex(index);
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



    void clearHoldedNote()
    {
        noteHolded = musicalNotes.None;
    }

    void addHoldedNote()
    {
        if (playedPartition.Count > 0)
        {
            noteHolded = playedPartition.Last();
        }
    }

    public int GetLastNoteIndex()
    {
        if (playedPartition.Count > 0)
        {
            if (playedPartition.Last() == musicalNotes.Do)
            {
                return 0;
            }
            if (playedPartition.Last() == musicalNotes.Ré)
            {
                return 1;
            }
            if (playedPartition.Last() == musicalNotes.Mi)
            {
                return 2;
            }
            if (playedPartition.Last() == musicalNotes.Fa)
            {
                return 3;
            }
            if (playedPartition.Last() == musicalNotes.Sol)
            {
                return 4;
            }
            if (playedPartition.Last() == musicalNotes.La)
            {
                return 5;
            }
            if (playedPartition.Last() == musicalNotes.Si)
            {
                return 6;
            }
            if (playedPartition.Last() == musicalNotes.Do2)
            {
                return 7;
            }
            if (playedPartition.Last() == musicalNotes.None)
            {
                return -1;
            }
        }
        return -1;
    }
    public bool PlayerHoldLastNote(musicalNotes note)
    {
        if(noteHolded == note)
        {
            return true;
        }
        return false;
            
    }

    //Ajout théo
    public bool HasJustPlayed(musicalNotes note)
    {

        return noteCurrent == note && Input.GetAxisRaw("ValidateNote") > 0;
    }

}