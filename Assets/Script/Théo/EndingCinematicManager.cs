using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

[System.Serializable]
public class FocusChange
{
    public float triggerWaypointIndex;
    public Transform newLookAt;
    public float transitionDuration = 1f;
    [Space(10)]
    public bool revertFocus;
    public float keepFocusForWaypoints = 1f;
    public Transform revertLookAtTarget;
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

    [Header("UI Fade Settings")]
    public CanvasGroup[] uiElementsToFadeIn;
    public float uiFadeDuration = 2f;

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
        // CHANGEMENT 1 : On met l'alpha à 0 mais on les laisse INACTIFS pour le moment
        foreach (var ui in uiElementsToFadeIn)
        {
            if (ui != null)
            {
                ui.alpha = 0f;
                ui.gameObject.SetActive(false);
            }
        }

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

            SetCameraFocus(sequenceCam, shot.lookAtTarget);

            shot.onShotStart?.Invoke();

            float timer = 0f;
            List<FocusChange> pendingFocusChanges = new List<FocusChange>(shot.focusChanges);

            CinemachineDollyCart cart = shot.cameraPoint.GetComponent<CinemachineDollyCart>();
            bool isAtEndOfTrack = false;
            float endTrackTimer = 0f;
            float maxCartPos = 0f;
            bool isFadingUI = false;
            bool isWaitingForInput = false;
            float fadeTimer = 0f;
            int currentUiIndex = 0;

            GameObject proxyObj = new GameObject("Ending_LookAtProxy");
            Transform currentLookTarget = shot.lookAtTarget;
            Transform previousLookTarget = shot.lookAtTarget;
            bool isTransitioning = false;
            float transitionTimer = 0f;
            float currentTransitionDuration = 0f;

            if (cart != null && cart.m_Path != null)
            {
                if (cart.m_PositionUnits == CinemachinePathBase.PositionUnits.Normalized)
                    maxCartPos = 1f;
                else if (cart.m_PositionUnits == CinemachinePathBase.PositionUnits.Distance)
                    maxCartPos = cart.m_Path.PathLength;
                else
                    maxCartPos = cart.m_Path.MaxPos;
            }

            while (true)
            {
                timer += Time.deltaTime;
                float currentWaypointPos = 0f;

                if (cart != null && cart.m_Path != null)
                {
                    currentWaypointPos = cart.m_Path.ToNativePathUnits(cart.m_Position, cart.m_PositionUnits);
                }

                for (int i = pendingFocusChanges.Count - 1; i >= 0; i--)
                {
                    FocusChange focus = pendingFocusChanges[i];

                    if (currentWaypointPos >= focus.triggerWaypointIndex)
                    {
                        previousLookTarget = currentLookTarget;
                        currentLookTarget = focus.newLookAt;
                        currentTransitionDuration = focus.transitionDuration;

                        if (currentTransitionDuration > 0f)
                        {
                            isTransitioning = true;
                            transitionTimer = 0f;
                            SetCameraFocus(sequenceCam, proxyObj.transform);
                        }
                        else
                        {
                            isTransitioning = false;
                            SetCameraFocus(sequenceCam, currentLookTarget);
                        }

                        if (focus.revertFocus)
                        {
                            FocusChange revertChange = new FocusChange
                            {
                                triggerWaypointIndex = focus.triggerWaypointIndex + focus.keepFocusForWaypoints,
                                newLookAt = focus.revertLookAtTarget,
                                transitionDuration = focus.transitionDuration,
                                revertFocus = false
                            };
                            pendingFocusChanges.Add(revertChange);
                        }

                        pendingFocusChanges.RemoveAt(i);
                    }
                }

                if (isTransitioning)
                {
                    Vector3 startPos = previousLookTarget != null ? previousLookTarget.position : shot.cameraPoint.position + shot.cameraPoint.forward * 30f;
                    Vector3 endPos = currentLookTarget != null ? currentLookTarget.position : shot.cameraPoint.position + shot.cameraPoint.forward * 30f;

                    transitionTimer += Time.deltaTime;
                    float t = Mathf.Clamp01(transitionTimer / currentTransitionDuration);
                    t = t * t * (3f - 2f * t);

                    proxyObj.transform.position = Vector3.Lerp(startPos, endPos, t);

                    if (transitionTimer >= currentTransitionDuration)
                    {
                        isTransitioning = false;
                        SetCameraFocus(sequenceCam, currentLookTarget);
                    }
                }

                if (cart != null && cart.m_Path != null && !isAtEndOfTrack)
                {
                    if (cart.m_Position >= maxCartPos)
                    {
                        isAtEndOfTrack = true;
                    }
                }

                if (isAtEndOfTrack && !isFadingUI)
                {
                    endTrackTimer += Time.deltaTime;
                    if (endTrackTimer >= 3f)
                    {
                        isFadingUI = true;
                    }
                }

                // --- GESTION DES FADES UI DE FIN ---
                if (isFadingUI)
                {
                    if (currentUiIndex < uiElementsToFadeIn.Length)
                    {
                        CanvasGroup currentUi = uiElementsToFadeIn[currentUiIndex];

                        if (currentUi != null)
                        {
                            // CHANGEMENT 2 : On active l'élément uniquement au moment où son fondu commence
                            if (!currentUi.gameObject.activeSelf)
                            {
                                currentUi.gameObject.SetActive(true);
                            }

                            fadeTimer += Time.deltaTime;
                            float alpha = Mathf.Clamp01(fadeTimer / uiFadeDuration);
                            currentUi.alpha = alpha;

                            if (fadeTimer >= uiFadeDuration)
                            {
                                currentUi.alpha = 1f; // Sécurité alpha max

                                // CHANGEMENT 3 : LE FADE EST FINI, ON COMMENCE LE SCROLL ICI !
                                CreditsManager credits = currentUi.GetComponentInChildren<CreditsManager>();
                                if (credits != null)
                                {
                                    credits.StartCredits();
                                }

                                fadeTimer = 0f;
                                currentUiIndex++;
                            }
                        }
                        else
                        {
                            currentUiIndex++;
                        }
                    }
                    else
                    {
                        isWaitingForInput = true;
                    }
                }
                else if (!isAtEndOfTrack && timer >= shot.duration)
                {
                    break;
                }

                if (isWaitingForInput && Input.anyKeyDown)
                {
                    onCinematicFinished?.Invoke();
                    cinematicRoutine = null;
                    yield break;
                }

                yield return null;
            }

            Destroy(proxyObj);
            Destroy(camObj);
        }

        onCinematicFinished?.Invoke();
        cinematicRoutine = null;
    }

    private void SetCameraFocus(CinemachineVirtualCamera cam, Transform target)
    {
        if (cam == null) return;

        cam.LookAt = target;

        if (target != null)
        {
            var composer = cam.GetCinemachineComponent<CinemachineComposer>();
            if (composer == null)
            {
                var sameAsFollow = cam.GetCinemachineComponent<CinemachineSameAsFollowTarget>();
                if (sameAsFollow != null) Destroy(sameAsFollow);

                composer = cam.AddCinemachineComponent<CinemachineComposer>();
                composer.m_TrackedObjectOffset = Vector3.zero;
            }
        }
        else
        {
            var composer = cam.GetCinemachineComponent<CinemachineComposer>();
            if (composer != null) Destroy(composer);

            var sameAsFollow = cam.GetCinemachineComponent<CinemachineSameAsFollowTarget>();
            if (sameAsFollow == null)
            {
                cam.AddCinemachineComponent<CinemachineSameAsFollowTarget>();
            }
        }
    }
}