using UnityEngine;
using System.Collections;

public class MoveToTarget : MonoBehaviour
{
    public Transform target;
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
        if (enigmeSystem == null || target == null) return;

        if (enigmeSystem.ratio > previousRatio)
        {
            MoveToRatio(enigmeSystem.ratio);
        }
        else if (enigmeSystem.ratio == 0 && previousRatio > 0)
        {
            MoveToRatio(0f);
        }

        previousRatio = enigmeSystem.ratio;
    }

    public void MoveToRatio(float targetRatio)
    {
        Vector3 targetPos = Vector3.Lerp(initialPos, target.position, targetRatio);
        Quaternion targetRot = Quaternion.Lerp(initialRot, target.rotation, targetRatio);
        Vector3 targetScale = Vector3.Lerp(initialScale, target.localScale, targetRatio);

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveRoutine(targetPos, targetRot, targetScale));
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