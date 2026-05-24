using System.Collections;
using UnityEngine;
using Cinemachine;

public class PropPusher : MonoBehaviour
{
    public Transform finalState;
    public float pushDuration = 5f;

    [Header("Cinematic Camera")]
    public bool enableCinematicCamera = true;
    public Transform cinematicCameraPoint;
    public float cameraFocusDuration = 2f;

    public GameObject objectToDeactivateOnStart;

    private CinemachineVirtualCamera sequenceCam;

    public IEnumerator PushRoutine()
    {
        if (objectToDeactivateOnStart != null)
        {
            objectToDeactivateOnStart.SetActive(false);
        }

        if (enableCinematicCamera && cinematicCameraPoint != null)
        {
            GameObject camObj = new GameObject("Pusher_CinematicCam");
            camObj.transform.position = cinematicCameraPoint.position;

            sequenceCam = camObj.AddComponent<CinemachineVirtualCamera>();
            sequenceCam.Follow = cinematicCameraPoint;
            sequenceCam.LookAt = this.transform;
            sequenceCam.Priority = 100;

            var transposer = sequenceCam.AddCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = Vector3.zero;

            var composer = sequenceCam.AddCinemachineComponent<CinemachineComposer>();
            composer.m_TrackedObjectOffset = new Vector3(0, 0.5f, 0);

            StartCoroutine(StopCameraTimer(cameraFocusDuration));
        }

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

    private IEnumerator StopCameraTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (sequenceCam != null)
        {
            Destroy(sequenceCam.gameObject);
        }
    }
}