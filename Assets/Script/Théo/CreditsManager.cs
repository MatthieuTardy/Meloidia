using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CreditsManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI creditsText;

    public float fadeDuration = 2f;
    public float scrollSpeed = 50f;

    public UnityEvent onCreditsFinished;

    public void StartCredits()
    {
        // On initialise le groupe à 0
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(true);
        }

        // On lance les deux actions en même temps (en parallèle)
        StartCoroutine(FadeRoutine());
        StartCoroutine(ScrollRoutine());
    }

    // --- COROUTINE 1 : Gère uniquement l'apparition de 0 à 1 ---
    private IEnumerator FadeRoutine()
    {
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

        // Sécurité pour bien finir à 1
        if (canvasGroup != null) canvasGroup.alpha = 1f;
    }

    // --- COROUTINE 2 : Gère l'attente puis le défilement ---
    private IEnumerator ScrollRoutine()
    {
        // 1. On met la coroutine en pause TANT QUE l'opacité est en dessous de 0.5
        while (canvasGroup != null && canvasGroup.alpha < 0.5f)
        {
            yield return null; // On attend la frame suivante
        }

        // --- À partir d'ici, l'opacité a atteint 0.5, le texte démarre ! ---

        // 2. Défilement du texte vers le haut
        if (creditsText != null)
        {
            RectTransform textRect = creditsText.rectTransform;
            float textHeight = creditsText.preferredHeight;
            float startY = textRect.anchoredPosition.y;
            float targetY = startY + textHeight + 500f;

            // Le texte défile tant qu'il n'a pas atteint sa cible
            while (textRect.anchoredPosition.y < targetY)
            {
                textRect.anchoredPosition += Vector2.up * (scrollSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // 3. Fin des crédits
        onCreditsFinished?.Invoke();
    }
}