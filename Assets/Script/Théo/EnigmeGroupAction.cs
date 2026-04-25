using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnigmeGroupAction : MonoBehaviour
{
    public CrocNoteCondition condition;
    public CrocNoteProximity proximity;

    [Header("Sequence Settings")]
    public Transform targetPoint;
    public PropPusher propToPush;

    [Header("Formation Settings")]
    public float spacing = 1.2f;       // Space between them left/right
    public int maxCrocNotesPerLine = 4; // How many on the first line before starting a new one
    public float rowSpacing = 1.2f;    // Space between the front row and back row
    public float rotationOffset = 0f;

    public UnityEvent onPropPushed;

    private int totalCrocNotes;
    private int arrivedCrocNotes;
    private List<DynamicActionSequence> activeSequences = new List<DynamicActionSequence>();
    private bool isActionRunning = false;

    private Coroutine failSafeCoroutine;

    public void TryTriggerAction()
    {
        if (isActionRunning) return;

        if (condition != null && condition.CheckCondition())
        {
            isActionRunning = true;

            List<LegumeManager> participants = new List<LegumeManager>(proximity.CrocNoteInProximity);

            totalCrocNotes = participants.Count;
            arrivedCrocNotes = 0;
            activeSequences.Clear();

            if (failSafeCoroutine != null) StopCoroutine(failSafeCoroutine);
            failSafeCoroutine = StartCoroutine(FailSafeTimer());

            int index = 0;
            foreach (LegumeManager crocNote in participants)
            {
                if (crocNote != null)
                {
                    proximity.CrocNoteInProximity.Remove(crocNote);

                    DynamicActionSequence sequence = crocNote.gameObject.AddComponent<DynamicActionSequence>();

                    // --- Grid Formation Calculation ---
                    int row = index / maxCrocNotesPerLine;
                    int col = index % maxCrocNotesPerLine;

                    int itemsInThisRow = Mathf.Min(maxCrocNotesPerLine, totalCrocNotes - (row * maxCrocNotesPerLine));

                    float offsetX = (col - (itemsInThisRow - 1) / 2f) * spacing;
                    float offsetZ = -row * rowSpacing; // Negative so rows build backwards

                    sequence.sequenceOffset = offsetX;
                    sequence.sequenceOffsetZ = offsetZ;

                    Vector3 desiredPos = targetPoint.position
                                       + (targetPoint.right * offsetX)
                                       + (targetPoint.forward * offsetZ);

                    if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
                    {
                        sequence.targetPosition = hit.position;
                    }
                    else
                    {
                        sequence.targetPosition = desiredPos;
                    }

                    sequence.manager = this;
                    activeSequences.Add(sequence);
                    sequence.StartSequence();

                    index++;
                }
            }
        }
    }

    private IEnumerator FailSafeTimer()
    {
        yield return new WaitForSeconds(15f);

        Debug.LogWarning("EnigmeGroupAction: Push took too long! Triggering Fail-Safe.");

        foreach (DynamicActionSequence seq in activeSequences)
        {
            if (seq != null)
            {
                seq.FinishAndReturn();
            }
        }
        activeSequences.Clear();

        if (propToPush != null)
        {
            yield return StartCoroutine(propToPush.PushRoutine());
        }

        onPropPushed?.Invoke();
        isActionRunning = false;
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
        if (failSafeCoroutine != null)
        {
            StopCoroutine(failSafeCoroutine);
            failSafeCoroutine = null;
        }

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

        isActionRunning = false;
    }
}