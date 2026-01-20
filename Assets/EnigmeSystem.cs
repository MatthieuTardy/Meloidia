using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;

public class EnigmeSystem : MonoBehaviour
{
    // Start is called before the first frame update
    private NoteSystem noteSystem;
    Coroutine waitRoutine;
    [SerializeField] List<musicalNotes> chantEnigme = new List<musicalNotes> { musicalNotes.Do, musicalNotes.Ré, musicalNotes.Mi };
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Chant")
        {
            if(waitRoutine == null)
            {
                waitRoutine = StartCoroutine(Chant());
                Debug.LogWarning("start chant");
            }
                

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Chant")
        {
            if (waitRoutine != null)
            {
                StopCoroutine(waitRoutine);
                Debug.LogWarning("stop chant");
            }


        }
    }
    IEnumerator Chant()
    {
        Debug.LogWarning("AAAAAAAAAAAAAAAAAA");
        yield return new WaitUntil(() => GameManager.Instance.playerManager.noteSystem.PlayerSingCorrectPattern(chantEnigme));
        Debug.LogWarning("BBBBBBBBBBBBBBBB");
        RuntimeManager.PlayOneShot("event:/Win");
    }
}
