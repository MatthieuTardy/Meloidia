using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMODUnity;


public class NoteSystem : MonoBehaviour
{

    float playedTime;
    string noteBefore;

    public StudioEventEmitter DO;
    public StudioEventEmitter RE;
    public StudioEventEmitter MI;
    public StudioEventEmitter FA;
    public StudioEventEmitter SOL;
    public StudioEventEmitter LA;
    public StudioEventEmitter SI;
    public StudioEventEmitter DO2;


    public string[] musicalNotes = new string[8]
    {
        "Do (Nord)",
        "Ré (Nord-Est)",
        "Mi (Est)",
        "Fa (Sud-Est)",
        "Sol (Sud)",
        "La (Sud-Ouest)",
        "Si (Ouest)",
        "Do' (Nord-Ouest)"
    };
    public List<string> listeDeNotes = new List<string>
    {
        "Do (Nord)",
        "Ré (Nord-Est)",
        "Mi (Est)"
    };
    public List<string> listeDeNotes2 = new List<string>
    {
        "Do (Nord)",
        "Ré (Nord-Est)",
        "Do (Nord)",
        "Mi (Est)"
    };
    public List<string> playedPartition;

    private void Start()
    {

    }

    void Update()
    {
        playedTime += Time.deltaTime;
        PlayMusic();
        if (playedTime >= 3)
        {
            for (var i = 0; i < playedPartition.Count; i++)
            {
                playedPartition.RemoveAt(i);
            }
        }
    }

    private void PlayMusic()
    {
        if (Input.GetAxis("SongX_Xbox") > 0.5f || Input.GetAxis("SongY_Xbox") > 0.5f || Input.GetAxis("SongX_Xbox") < -0.5f || Input.GetAxis("SongY_Xbox") < -0.5f)
        {
            int noteIndex = 0;
            if ((Input.GetAxis("SongY_Xbox") > 0.5f) && Input.GetAxis("SongX_Xbox") < 0.5f && Input.GetAxis("SongX_Xbox") > -0.5f) { noteIndex = 4; } // Nord
            else if ((Input.GetAxis("SongX_Xbox") > 0.3f) && Input.GetAxis("SongY_Xbox") > 0.3f) { noteIndex = 3; } // Nord-Est
            else if ((Input.GetAxis("SongX_Xbox") > 0.5f) && Input.GetAxis("SongY_Xbox") < 0.5f && Input.GetAxis("SongY_Xbox") > -0.5f) { noteIndex = 2; } // Est
            else if ((Input.GetAxis("SongX_Xbox") > 0.3f) && Input.GetAxis("SongY_Xbox") < -0.3f) { noteIndex = 1; } // Sud-Est
            else if ((Input.GetAxis("SongY_Xbox") < -0.5f) && Input.GetAxis("SongX_Xbox") < 0.5f && Input.GetAxis("SongX_Xbox") > -0.5f) { noteIndex = 0; } // Sud
            else if ((Input.GetAxis("SongX_Xbox") < -0.3f) && Input.GetAxis("SongY_Xbox") < -0.3f) { noteIndex = 7; } // Sud-Ouest
            else if ((Input.GetAxis("SongX_Xbox") < -0.5f) && Input.GetAxis("SongY_Xbox") < 0.5f && Input.GetAxis("SongY_Xbox") > -0.5f) { noteIndex = 6; } // Ouest
            else if ((Input.GetAxis("SongX_Xbox") < -0.3f) && Input.GetAxis("SongY_Xbox") > 0.3f) { noteIndex = 5; } // Nord-Ouest       
            PlayNote(noteIndex);
        }
        else
            StopChant();
    }

    void StopChant()
    {
        DO.Stop();
        RE.Stop();
        MI.Stop();
        FA.Stop();
        SOL.Stop();
        LA.Stop();
        SI.Stop();
        DO2.Stop();
    }

    void StartChant(int index)
    {
        if (index == 0)
        {
            StopChant();
            DO.Play();
        }
        if (index == 1)
        {
            StopChant();
            RE.Play();
        }
        if (index == 2)
        {
            StopChant();
            MI.Play();
        }
        if (index == 3)
        {
            StopChant();
            FA.Play();
        }
        if (index == 4)
        {
            StopChant();
            SOL.Play();
        }
        if (index == 5)
        {
            StopChant();
            LA.Play();
        }
        if (index == 6)
        {
            StopChant();
            SI.Play();
        }
        if (index == 7)
        {
            StopChant();
            DO2.Play();
        }
    }
    void PlayNote(int index)
    {

        if (musicalNotes[index] != noteBefore || playedTime > 3)
        {
            StartChant(index);
            playedPartition.Add(musicalNotes[index]);
            Debug.Log(playedPartition[playedPartition.Count-1]);
            if (playedPartition.SequenceEqual(listeDeNotes))
            {
                Debug.LogWarning("Chant du diabète");
            }
            if (playedPartition.SequenceEqual(listeDeNotes2))
            {
                Debug.LogWarning("Chant du Bonheur");
            }

        }
        playedTime = 0;
        noteBefore = musicalNotes[index];
        string note = musicalNotes[index];

    }
}