using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public ScrollRect scrollRect;
    public float fadeDuration = 2f;
    public float delayBeforeScroll = 2f;
    public float scrollSpeed = 0.05f;
    public UnityEvent onCreditsFinished;

    public void StartCredits()
    {
        StartCoroutine(CreditsRoutine());
    }

    private IEnumerator CreditsRoutine()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(true);
        }

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            }
            yield return null;
        }

        yield return new WaitForSeconds(delayBeforeScroll);

        if (scrollRect != null)
        {
            while (scrollRect.verticalNormalizedPosition > 0f)
            {
                scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime;
                yield return null;
            }
        }

        onCreditsFinished?.Invoke();
    }
}