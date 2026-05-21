using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Events;
using System.Linq;

public class ProgressEnigmeSystem : MonoBehaviour
{
    [SerializeField] List<musicalNotes> chantEnigme = new List<musicalNotes> { musicalNotes.Do, musicalNotes.Ré, musicalNotes.Mi };
    [SerializeField] UnityEvent onEnigmeResolve;
    [SerializeField] UnityEvent onEnigmeStep;

    public float ratio;
    private bool isResolved;
    private Coroutine songCoroutine;
    private int currentStep = 0;


    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("trigger");
        if (other.gameObject.layer == 8 && !isResolved && songCoroutine == null)
        {
            Debug.Log("joueur");
            GameManager.Instance.playerManager.noteSystem.ClearPartition();
            songCoroutine = StartCoroutine(ChantLogic());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8 && songCoroutine != null)
        {
            StopCoroutine(songCoroutine);
            songCoroutine = null;
            currentStep = 0;
        }
    }

    IEnumerator ChantLogic()
    {
        int totalNotes = chantEnigme.Count;
        currentStep = 0;

        ///Théo Modif 
        var noteSystem = GameManager.Instance.playerManager.noteSystem;
        int lastNoteCount = noteSystem.playedPartition.Count;
        ///Théo Modif

        while (currentStep < totalNotes)
        {
            if (noteSystem.playedPartition.Count > 0 && !isResolved)
            {
                musicalNotes noteAttendue = chantEnigme[currentStep];

                ///Théo Modif
                yield return new WaitUntil(() => noteSystem.playedPartition.Count > lastNoteCount);

                lastNoteCount = noteSystem.playedPartition.Count;
                ///Théo Modif

                if (noteSystem.HasJustPlayed(noteAttendue))
                {
                    currentStep++;
                    ratio = (float)currentStep / totalNotes;
                    onEnigmeStep.Invoke();
                }
                else
                {
                    ///Théo Modif
                    if (totalNotes > 1)
                    {
                        currentStep = 0;
                        ratio = 0f;
                        onEnigmeStep.Invoke();
                    }
                    ///Théo Modif
                }

                if (currentStep >= totalNotes)
                {
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        isResolved = true;
        RuntimeManager.PlayOneShot("event:/Musics/Win");
        onEnigmeResolve.Invoke();

        songCoroutine = null;
    }
}