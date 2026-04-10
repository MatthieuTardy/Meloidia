using UnityEngine;
using System.Collections;

public class MoveToTarget : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    public float rotationSpeed = 200f;
    public bool useSmoothLerp = false;

    private Coroutine moveCoroutine;

    public void StartMoving()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned!");
            return;
        }

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveRoutine());
    }

    public void StartMovingTo(Transform newTarget)
    {
        target = newTarget;
        StartMoving();
    }

    private IEnumerator MoveRoutine()
    {
        while (Vector3.Distance(transform.position, target.position) > 0.01f ||
               Quaternion.Angle(transform.rotation, target.rotation) > 0.1f ||
               Vector3.Distance(transform.localScale, target.localScale) > 0.01f)
        {
            if (useSmoothLerp)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, speed * Time.deltaTime);
                transform.localScale = Vector3.Lerp(transform.localScale, target.localScale, speed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, rotationSpeed * Time.deltaTime);
                transform.localScale = Vector3.MoveTowards(transform.localScale, target.localScale, speed * Time.deltaTime);
            }

            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;
        transform.localScale = target.localScale;

        moveCoroutine = null;

        Debug.Log("Target transform fully matched!");
    }

    [ContextMenu("Test Move (Editor Only)")]
    private void TestMove()
    {
        StartMoving();
    }
}