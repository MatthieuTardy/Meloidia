using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public Material transitionMaterial;
    public float transitionDuration = 1.2f;
    public AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    // Effets additionnels
    public bool useScreenFlash = true;
    public Color flashColor = Color.white;
    public float flashIntensity = 0.5f;
    public bool useSoundEffect = true;
    public AudioClip transitionSound;

    private CanvasGroup fadeGroup;
    private AudioSource audioSource;
    private static ChangeScene instance;

    private void Awake()
    {
       
    }

    private void Start()
    {
        if (transitionMaterial != null)
        {
            // Met le radius à 0.0001f dès le démarrage (rempli)
            transitionMaterial.SetFloat("_Radius", 0.0001f);
        }

        fadeGroup = GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();

        // Si on vient d'une transition, on joue l'inverse
        StartCoroutine(PlayReverseTransition());
    }

    public void SceneChange(int index)
    {
        StartCoroutine(TransitionToScene(index));
    }

    private IEnumerator TransitionToScene(int sceneIndex)
    {
        // Joue le son de transition
        if (useSoundEffect && audioSource && transitionSound)
        {
            audioSource.PlayOneShot(transitionSound);
        }

        float elapsedTime = 0f;
        
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / transitionDuration;
            
            // Utilise la courbe d'animation pour un effet plus smooth
            float easedProgress = easingCurve.Evaluate(progress);
            float transitionValue = Mathf.Lerp(0.8f, 0.0001f, easedProgress);
            
            // Animation du radius (remplissage du cercle)
            transitionMaterial.SetFloat("_Radius", transitionValue);
            
            // Flash d'écran au milieu de l'animation
            if (useScreenFlash && fadeGroup)
            {
                float flashPeak = Mathf.Sin(progress * Mathf.PI);
                fadeGroup.alpha = flashPeak * flashIntensity;
            }
            
            transitionMaterial.SetFloat("_TransitionProgress", easedProgress);
            
            yield return null;
        }
        
        // Finalisation
        transitionMaterial.SetFloat("_Radius", 0.0001f);
        if (fadeGroup) fadeGroup.alpha = 0f;
        
        // Change la scène
        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator PlayReverseTransition()
    {
        // Petite pause pour laisser la scène se charger
        yield return new WaitForSeconds(0.3f);

        // Joue le son de transition
        if (useSoundEffect && audioSource && transitionSound)
        {
            audioSource.PlayOneShot(transitionSound);
        }

        float elapsedTime = 0f;
        
        // Animation inverse: 0.0001f (rempli) → 0.8f (vide)
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / transitionDuration;
            
            float easedProgress = easingCurve.Evaluate(progress);
            float transitionValue = Mathf.Lerp(0.0001f, 0.8f, easedProgress); // INVERSE!
            
            // Animation du radius (déverrouillage du cercle)
            transitionMaterial.SetFloat("_Radius", transitionValue);
            
            // Flash inverse
            if (useScreenFlash && fadeGroup)
            {
                float flashPeak = Mathf.Sin(progress * Mathf.PI);
                fadeGroup.alpha = flashPeak * flashIntensity;
            }
            
            transitionMaterial.SetFloat("_TransitionProgress", easedProgress);
            
            yield return null;
        }
        
        // Finalisation
        transitionMaterial.SetFloat("_Radius", 0.8f);
        if (fadeGroup) fadeGroup.alpha = 0f;
    }
}