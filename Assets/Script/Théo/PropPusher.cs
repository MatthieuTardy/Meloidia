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

        while (t < 1f)
        {
            t += Time.deltaTime / pushDuration;
            float normalizedTime = Mathf.Clamp01(t);

            transform.position = Vector3.Lerp(startPos, finalState.position, normalizedTime);
            transform.rotation = Quaternion.Lerp(startRot, finalState.rotation, normalizedTime);

            yield return null;
        }

        transform.position = finalState.position;
        transform.rotation = finalState.rotation;
    }
}