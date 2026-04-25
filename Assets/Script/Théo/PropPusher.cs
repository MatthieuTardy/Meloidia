using System.Collections;
using UnityEngine;

public class PropPusher : MonoBehaviour
{
    public Transform finalState;
    public float pushDuration = 2f;

    public IEnumerator PushRoutine()
    {
        float t = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 targetPos = finalState.position;
        Quaternion targetRot = finalState.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime / pushDuration;
            float normalizedTime = Mathf.Clamp01(t);

            transform.position = Vector3.Lerp(startPos, targetPos, normalizedTime);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, normalizedTime);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }
}