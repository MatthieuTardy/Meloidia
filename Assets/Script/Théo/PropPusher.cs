using System.Collections;
using UnityEngine;
using Cinemachine; // Required for the camera

public class PropPusher : MonoBehaviour
{
    public Transform finalState;
    public float pushDuration = 5f; // How long the actual push takes

    [Header("Cinematic Camera")]
    public bool enableCinematicCamera = true;
    public Transform cinematicCameraPoint;
    public float cameraFocusDuration = 2f; // How long the camera watches before returning to player

    private CinemachineVirtualCamera sequenceCam;

    public IEnumerator PushRoutine()
    {
        // --- 1. SETUP CINEMATIC CAMERA ---
        if (enableCinematicCamera && cinematicCameraPoint != null)
        {
            GameObject camObj = new GameObject("Pusher_CinematicCam");
            camObj.transform.position = cinematicCameraPoint.position;

            sequenceCam = camObj.AddComponent<CinemachineVirtualCamera>();
            sequenceCam.Follow = cinematicCameraPoint;
            sequenceCam.LookAt = this.transform; // Focuses on the prop being pushed
            sequenceCam.Priority = 100;

            var transposer = sequenceCam.AddCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = Vector3.zero;

            var composer = sequenceCam.AddCinemachineComponent<CinemachineComposer>();
            composer.m_TrackedObjectOffset = new Vector3(0, 0.5f, 0);

            // Start a background timer to turn the camera off after exactly 2 seconds
            StartCoroutine(StopCameraTimer(cameraFocusDuration));
        }

        // --- 2. ORIGINAL PUSH LOGIC ---
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

        // Ensure it snaps perfectly to the end when finished
        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    // --- 3. BACKGROUND CAMERA TIMER ---
    // This routine runs independently. It waits for the specified time, 
    // then destroys the camera so Cinemachine smoothly transitions back to the player.
    private IEnumerator StopCameraTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (sequenceCam != null)
        {
            Destroy(sequenceCam.gameObject);
        }
    }
}