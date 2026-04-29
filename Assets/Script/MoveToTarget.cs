using UnityEngine;
using System.Collections;

public class MoveToTarget : MonoBehaviour
{
    public Transform[] targets;
    public float speed = 5f;
    public ProgressEnigmeSystem enigmeSystem;

    private Vector3 initialPos;
    private Quaternion initialRot;
    private Vector3 initialScale;
    private bool hasInitPos = false;

    private float previousRatio = 0f;
    private Coroutine moveCoroutine;

    void Start()
    {
        if (!hasInitPos)
        {
            initialPos = transform.position;
            initialRot = transform.rotation;
            initialScale = transform.localScale;
            hasInitPos = true;
        }

        if (enigmeSystem != null)
        {
            previousRatio = enigmeSystem.ratio;
        }
    }

    void Update()
    {
        if (enigmeSystem == null || targets == null || targets.Length == 0) return;

        if (enigmeSystem.ratio > previousRatio)
        {
            MoveToRatio(enigmeSystem.ratio);
        }
        else if (enigmeSystem.ratio == 0 && previousRatio > 0)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            GetTargetState(0f, out Vector3 targetPos, out Quaternion targetRot, out Vector3 targetScale);
            transform.position = targetPos;
            transform.rotation = targetRot;
            transform.localScale = targetScale;
        }

        previousRatio = enigmeSystem.ratio;
    }

    public void MoveToRatio(float targetRatio)
    {
        GetTargetState(targetRatio, out Vector3 targetPos, out Quaternion targetRot, out Vector3 targetScale);

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveRoutine(targetPos, targetRot, targetScale));
    }

    private void GetTargetState(float ratio, out Vector3 outPos, out Quaternion outRot, out Vector3 outScale)
    {
        if (ratio <= 0f)
        {
            outPos = initialPos; outRot = initialRot; outScale = initialScale;
            return;
        }

        if (ratio >= 1f)
        {
            Transform lastTarget = targets[targets.Length - 1];
            outPos = lastTarget.position; outRot = lastTarget.rotation; outScale = lastTarget.localScale;
            return;
        }

        int totalSegments = targets.Length;
        float continuousIndex = ratio * totalSegments;
        int index = Mathf.Clamp(Mathf.FloorToInt(continuousIndex), 0, totalSegments - 1);
        float segmentRatio = continuousIndex - index;

        Vector3 startPos = (index == 0) ? initialPos : targets[index - 1].position;
        Quaternion startRot = (index == 0) ? initialRot : targets[index - 1].rotation;
        Vector3 startScale = (index == 0) ? initialScale : targets[index - 1].localScale;

        Transform endTarget = targets[index];

        outPos = Vector3.Lerp(startPos, endTarget.position, segmentRatio);
        outRot = Quaternion.Lerp(startRot, endTarget.rotation, segmentRatio);
        outScale = Vector3.Lerp(startScale, endTarget.localScale, segmentRatio);
    }

    private IEnumerator MoveRoutine(Vector3 tPos, Quaternion tRot, Vector3 tScale)
    {
        while (Vector3.Distance(transform.position, tPos) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, tPos, speed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, tRot, speed * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, tScale, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = tPos;
        transform.rotation = tRot;
        transform.localScale = tScale;
    }
}