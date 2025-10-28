using UnityEngine;


public class WandersongNoteSystem : MonoBehaviour
{


    // Les 8 notes, dans l'ordre de la logique angulaire du code : Nord, NE, Est, SE, Sud, SO, Ouest, NO
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

    void Update()
    {

        if (Input.GetAxis("SongX_Xbox") > 0.5f || Input.GetAxis("SongY_Xbox") > 0.5f|| Input.GetAxis("SongX_Xbox") < -0.5f || Input.GetAxis("SongY_Xbox") < -0.5f)
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

            // 4. Déclencher l'action
            PlayNote(noteIndex);
        }
    }

    void PlayNote(int index)
    {
        string note = musicalNotes[index];
        Debug.Log($"Note Jouée : {note} (Index {index})");


    }
}