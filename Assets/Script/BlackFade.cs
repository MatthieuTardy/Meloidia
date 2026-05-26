using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackFade : MonoBehaviour
{
    public float fadeDuration = 1f;
    public Color fadeColor = Color.black;
    private Image fadeImage;

    private void Start()
    {
        fadeImage = GetComponent<Image>();
        if (fadeImage == null)
        {
            Debug.LogError("BlackFade: Image component not found!");
            return;
        }

        // Démarre avec un écran noir
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);

        // Joue le fondu (noir → transparent)
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;

            // Fade de 1 (opaque) à 0 (transparent)
            Color newColor = fadeImage.color;
            newColor.a = Mathf.Lerp(1f, 0f, progress);
            fadeImage.color = newColor;

            yield return null;
        }

        // Finalisation
        Color finalColor = fadeImage.color;
        finalColor.a = 0f;
        fadeImage.color = finalColor;
    }
}