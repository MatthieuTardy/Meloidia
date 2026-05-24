using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

[System.Serializable]
public class FocusChange
{
    public float timeDelay;
    public Transform newLookAt;
}

[System.Serializable]
public class CinematicShot
{
    public Transform cameraPoint;
    public Transform lookAtTarget;
    public float duration = 3f;
    public UnityEvent onShotStart;
    public FocusChange[] focusChanges;
}

public class EndingCinematicManager : MonoBehaviour
{
    public CinematicShot[] cinematicShots;
    public UnityEvent onCinematicFinished;

    private Coroutine cinematicRoutine;

    public void PlayEndingCinematic()
    {
        if (cinematicRoutine == null)
        {
            cinematicRoutine = StartCoroutine(CinematicRoutine());
        }
    }

    private IEnumerator CinematicRoutine()
    {
        foreach (var shot in cinematicShots)
        {
            if (shot.cameraPoint == null) continue;

            GameObject camObj = new GameObject("Ending_CinematicCam");
            camObj.transform.position = shot.cameraPoint.position;
            camObj.transform.rotation = shot.cameraPoint.rotation;

            CinemachineVirtualCamera sequenceCam = camObj.AddComponent<CinemachineVirtualCamera>();
            sequenceCam.Follow = shot.cameraPoint;
            sequenceCam.Priority = 100;

            var transposer = sequenceCam.AddCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = Vector3.zero;

            if (shot.lookAtTarget != null)
            {
                sequenceCam.LookAt = shot.lookAtTarget;
                var composer = sequenceCam.AddCinemachineComponent<CinemachineComposer>();
                composer.m_TrackedObjectOffset = Vector3.zero;
            }
            else
            {
                sequenceCam.AddCinemachineComponent<CinemachineSameAsFollowTarget>();
            }

            shot.onShotStart?.Invoke();

            float timer = 0f;
            List<FocusChange> pendingFocusChanges = new List<FocusChange>(shot.focusChanges);

            while (timer < shot.duration)
            {
                timer += Time.deltaTime;

                for (int i = pendingFocusChanges.Count - 1; i >= 0; i--)
                {
                    if (timer >= pendingFocusChanges[i].timeDelay)
                    {
                        if (sequenceCam != null && pendingFocusChanges[i].newLookAt != null)
                        {
                            sequenceCam.LookAt = pendingFocusChanges[i].newLookAt;

                            var composer = sequenceCam.GetCinemachineComponent<CinemachineComposer>();
                            if (composer == null)
                            {
                                var sameAsFollow = sequenceCam.GetComponent<CinemachineSameAsFollowTarget>();
                                if (sameAsFollow != null) Destroy(sameAsFollow);

                                composer = sequenceCam.AddCinemachineComponent<CinemachineComposer>();
                                composer.m_TrackedObjectOffset = Vector3.zero;
                            }
                        }
                        pendingFocusChanges.RemoveAt(i);
                    }
                }

                yield return null;
            }

            Destroy(camObj);
        }

        Debug.Log("Cinematique Fin");
        onCinematicFinished?.Invoke();
        cinematicRoutine = null;
    }
}