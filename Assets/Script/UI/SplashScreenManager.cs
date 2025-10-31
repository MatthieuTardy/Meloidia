using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // MODIFIÉ : Ajout du namespace pour TextMeshPro

public class SplashScreenManager : MonoBehaviour
{
    // Référence à l'image du splash screen
    public Image splashImage;

    // MODIFIÉ : La variable est maintenant de type TextMeshProUGUI
    public TextMeshProUGUI introText;

    // Durées
    public float imageWaitTime = 3.0f;
    public float imageFadeDuration = 1.5f;
    public float delayAfterImage = 2.0f;
    public float textWaitTime = 3.0f;
    public float textFadeDuration = 1.5f;

    private bool isFadingImage = false;

    void Start()
    {
        // Préparer l'image
        splashImage.color = new Color(splashImage.color.r, splashImage.color.g, splashImage.color.b, 1);

        // Cacher le texte au début
        if (introText != null)
        {
            introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, 0);
        }

        StartCoroutine(SplashScreenCoroutine());
    }

    void Update()
    {
        // Déclenche le fondu de l'image si le joueur bouge
        if (!isFadingImage && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.anyKeyDown))
        {
            isFadingImage = true;
            StopAllCoroutines();
            StartCoroutine(FadeOutImage());
        }
    }

    private IEnumerator SplashScreenCoroutine()
    {
        yield return new WaitForSeconds(imageWaitTime);

        if (!isFadingImage)
        {
            isFadingImage = true;
            StartCoroutine(FadeOutImage());
        }
    }

    private IEnumerator FadeOutImage()
    {
        float elapsedTime = 0f;
        Color originalColor = splashImage.color;

        while (elapsedTime < imageFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / imageFadeDuration);
            splashImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        splashImage.gameObject.SetActive(false);

        // Démarrer la séquence du texte
        if (introText != null)
        {
            StartCoroutine(ShowAndFadeOutText());
        }
    }

    private IEnumerator ShowAndFadeOutText()
    {
        // Attendre après le fondu de l'image
        yield return new WaitForSeconds(delayAfterImage);

        // Faire apparaître le texte (fondu d'entrée)
        float elapsedTime = 0f;
        Color textColor = introText.color;
        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / textFadeDuration);
            introText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }

        introText.color = new Color(textColor.r, textColor.g, textColor.b, 1);

        // Attendre
        yield return new WaitForSeconds(textWaitTime);

        // Faire disparaître le texte (fondu de sortie)
        elapsedTime = 0f;
        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / textFadeDuration);
            introText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }

        introText.gameObject.SetActive(false);
    }
}