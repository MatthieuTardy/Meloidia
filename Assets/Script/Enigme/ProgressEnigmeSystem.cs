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
    private bool isFinish;
    private Coroutine waitRoutine;
    private int currentStep = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if (!isFinish)
            {
                if (waitRoutine != null)
                {
                    StopCoroutine(waitRoutine);
                }
                GameManager.Instance.playerManager.noteSystem.ClearPartition();
                waitRoutine = StartCoroutine(ChantLogic());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8 && waitRoutine != null)
        {
            if (!isFinish)
            {
                StopCoroutine(waitRoutine);
                waitRoutine = null;
                currentStep = 0;
            }
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
            if (GameManager.Instance.playerManager.noteSystem.playedPartition.Count > 0 && !isFinish)
            {
                musicalNotes noteAttendue = chantEnigme[currentStep];

                ///Théo Modif
                yield return new WaitUntil(() => noteSystem.playedPartition.Count > lastNoteCount);

                lastNoteCount = noteSystem.playedPartition.Count;
                ///Théo Modif

                if (GameManager.Instance.playerManager.noteSystem.HasJustPlayed(noteAttendue))
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
        isFinish = true;
        RuntimeManager.PlayOneShot("event:/Musics/Win");
        onEnigmeResolve.Invoke();



        waitRoutine = null;
    }
}