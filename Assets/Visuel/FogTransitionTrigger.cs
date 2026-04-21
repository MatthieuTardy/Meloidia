using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FogTransitionTrigger : MonoBehaviour
{
    [Header("Transition")]
    [SerializeField] private float transitionDuration = 2f;
    
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    
    [Header("Fog State 1")]
    [SerializeField] private float fog1Density = 0.05f;
    [SerializeField] private Color fog1Color = Color.gray;
    
    [Header("Fog State 2")]
    [SerializeField] private float fog2Density = 0.1f;
    [SerializeField] private Color fog2Color = Color.blue;
    
    [Header("Sky Color")]
    [SerializeField] private Color sky1Color = Color.cyan;
    [SerializeField] private Color sky2Color = Color.blue;
    
    [Header("Vignette")]
    [SerializeField] private float vignette1Intensity = 0.1f;
    [SerializeField] private float vignette2Intensity = 0.5f;
    
    [Header("Color Adjustments - State 1")]
    [SerializeField] private float colorAdjust1Saturation = 0f;
    [SerializeField] private float colorAdjust1Contrast = 0f;
    
    [Header("Color Adjustments - State 2")]
    [SerializeField] private float colorAdjust2Saturation = -50f;
    [SerializeField] private float colorAdjust2Contrast = 30f;

    private Volume globalVolume;
    private Vignette vignetteComponent;
    private ColorAdjustments colorAdjustmentsComponent;
    
    private Coroutine transitionCoroutine;
    private float currentProgress = 0f; // 0 = fog1, 1 = fog2
    private bool isTransitioningToFog2 = false;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        globalVolume = FindObjectOfType<Volume>();
        
        if (globalVolume != null)
        {
            globalVolume.profile.TryGet<Vignette>(out vignetteComponent);
            globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustmentsComponent);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            isTransitioningToFog2 = true;
            transitionCoroutine = StartCoroutine(TransitionFog());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            isTransitioningToFog2 = false;
            transitionCoroutine = StartCoroutine(TransitionFog());
        }
    }

    private IEnumerator TransitionFog()
    {
        while (true)
        {
            if (isTransitioningToFog2)
            {
                // Vers fog2
                if (currentProgress < 1f)
                {
                    currentProgress += Time.deltaTime / transitionDuration;
                    currentProgress = Mathf.Clamp01(currentProgress);
                }
            }
            else
            {
                // Vers fog1
                if (currentProgress > 0f)
                {
                    currentProgress -= Time.deltaTime / transitionDuration;
                    currentProgress = Mathf.Clamp01(currentProgress);
                }
            }

            ApplyTransition(currentProgress);

            yield return null;
        }
    }

    private void ApplyTransition(float t)
    {
        float fogDensity = Mathf.Lerp(fog1Density, fog2Density, t);
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogColor = Color.Lerp(fog1Color, fog2Color, t);

        if (mainCamera != null)
        {
            mainCamera.backgroundColor = Color.Lerp(sky1Color, sky2Color, t);
        }

        if (vignetteComponent != null)
        {
            vignetteComponent.intensity.value = Mathf.Lerp(vignette1Intensity, vignette2Intensity, t);
        }

        if (colorAdjustmentsComponent != null)
        {
            colorAdjustmentsComponent.saturation.value = Mathf.Lerp(colorAdjust1Saturation, colorAdjust2Saturation, t);
            colorAdjustmentsComponent.contrast.value = Mathf.Lerp(colorAdjust1Contrast, colorAdjust2Contrast, t);
        }
    }
}