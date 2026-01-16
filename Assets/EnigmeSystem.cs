using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmeSystem : MonoBehaviour
{
    // Start is called before the first frame update
    private NoteSystem noteSystem;
    void Start()
    {
        noteSystem = GetComponent<NoteSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        Debug.LogWarning(noteSystem.PlayerSingCorrectPattern(noteSystem.chantDuDiab));
        bool chant = noteSystem.PlayerSingCorrectPattern(noteSystem.chantDuDiab);
        if (chant == true && other.tag == "Enigme1")
        {
            Debug.LogWarning("Bravo !!!");
        }
        else if (chant == true)
        {
            Debug.LogWarning("bon");
        }
        else if (other.tag == "Enigme1")
        {
            Debug.LogWarning("TOUCHERRRRR");
        }
    }
}
