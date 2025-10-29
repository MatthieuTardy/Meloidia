using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoteSystem : MonoBehaviour
{

    float playedTime;
    string noteBefore;

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
            playedTime = 0;
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
            else if ((Input.GetAxis("SongX_Xbox") > 0.5f) && Input.GetAxis("SongY_Xbox") > 0.5f) { noteIndex = 3; } // Nord-Est
            else if ((Input.GetAxis("SongX_Xbox") > 0.5f) && Input.GetAxis("SongY_Xbox") < 0.5f && Input.GetAxis("SongY_Xbox") > -0.5f) { noteIndex = 2; } // Est
            else if ((Input.GetAxis("SongX_Xbox") > 0.5f) && Input.GetAxis("SongY_Xbox") < -0.5f) { noteIndex = 1; } // Sud-Est
            else if ((Input.GetAxis("SongY_Xbox") < -0.5f) && Input.GetAxis("SongX_Xbox") < 0.5f && Input.GetAxis("SongX_Xbox") > -0.5f) { noteIndex = 0; } // Sud
            else if ((Input.GetAxis("SongX_Xbox") < -0.5f) && Input.GetAxis("SongY_Xbox") < -0.5f) { noteIndex = 7; } // Sud-Ouest
            else if ((Input.GetAxis("SongX_Xbox") < -0.5f) && Input.GetAxis("SongY_Xbox") < 0.5f && Input.GetAxis("SongY_Xbox") > -0.5f) { noteIndex = 6; } // Ouest
            else if ((Input.GetAxis("SongX_Xbox") < -0.5f) && Input.GetAxis("SongY_Xbox") > 0.5f) { noteIndex = 5; } // Nord-Ouest

            PlayNote(noteIndex);
        }
    }

    void PlayNote(int index)
    {

        if (musicalNotes[index] != noteBefore && playedTime > 3)
        {
            playedPartition.Add(musicalNotes[index]);
            Debug.Log(playedPartition[-1]);
        }
        playedTime = 0;
        noteBefore = musicalNotes[index];
        string note = musicalNotes[index];
        Debug.Log($"Note Jouée : {note} (Index {index})");
    }
}