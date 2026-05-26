using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CreditsManager : MonoBehaviour
{
    public TextMeshProUGUI creditsText;
    public float scrollSpeed = 50f;
    public UnityEvent onCreditsFinished;

    // Cette fonction sera appelée par le EndingCinematicManager dès que le Fade In est fini
    public void StartCredits()
    {
        StartCoroutine(ScrollRoutine());
    }

    private IEnumerator ScrollRoutine()
    {
        if (creditsText != null)
        {
            RectTransform textRect = creditsText.rectTransform;
            float textHeight = creditsText.preferredHeight;
            float startY = textRect.anchoredPosition.y;

            // Point d'arrivée hors de l'écran avec marge de sécurité
            float targetY = startY + textHeight + 500f;

            // Défilement continu en continu sans s'arrêter
            while (textRect.anchoredPosition.y < targetY)
            {
                textRect.anchoredPosition += Vector2.up * (scrollSpeed * Time.deltaTime);
                yield return null;
            }
        }

        onCreditsFinished?.Invoke();
    }
}