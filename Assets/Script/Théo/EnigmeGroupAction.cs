using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnigmeGroupAction : MonoBehaviour
{
    public CrocNoteCondition condition;
    public CrocNoteProximity proximity;

    [Header("Sequence Settings")]
    public Transform targetPoint;
    public PropPusher propToPush;
    public float spacing = 1.2f;
    public UnityEvent onPropPushed;

    private int totalCrocNotes;
    private int arrivedCrocNotes;
    private List<DynamicActionSequence> activeSequences = new List<DynamicActionSequence>();

    public void TryTriggerAction()
    {
        if (condition.CheckCondition())
        {
            totalCrocNotes = proximity.CrocNoteInProximity.Count;
            arrivedCrocNotes = 0;
            activeSequences.Clear();

            int index = 0;
            foreach (LegumeManager crocNote in proximity.CrocNoteInProximity)
            {
                if (crocNote != null)
                {
                    crocNote.StopFollowingLocation();

                    DynamicActionSequence sequence = crocNote.gameObject.AddComponent<DynamicActionSequence>();

                    float offset = (index - (totalCrocNotes - 1) / 2f) * spacing;
                    sequence.targetPosition = targetPoint.position + (targetPoint.right * offset);
                    sequence.manager = this;

                    activeSequences.Add(sequence);
                    sequence.StartSequence();

                    index++;
                }
            }
            proximity.CrocNoteInProximity.Clear();
        }
    }

    public void OnCrocNoteArrived(DynamicActionSequence sequence)
    {
        arrivedCrocNotes++;

        if (arrivedCrocNotes >= totalCrocNotes)
        {
            StartCoroutine(ExecuteGroupAction());
        }
    }

    private IEnumerator ExecuteGroupAction()
    {
        yield return new WaitForSeconds(0.2f);

        foreach (DynamicActionSequence seq in activeSequences)
        {
            if (seq != null) seq.StartPushing();
        }

        if (propToPush != null)
        {
            yield return StartCoroutine(propToPush.PushRoutine());
        }

        onPropPushed?.Invoke();

        foreach (DynamicActionSequence seq in activeSequences)
        {
            if (seq != null) seq.FinishAndReturn();
        }
    }
}