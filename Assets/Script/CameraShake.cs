using System.Collections;
using UnityEngine;
using Cinemachine;


public class CameraShake : MonoBehaviour
{
    [Header("Cinemachine")]
    [Tooltip("Source d'impulsion utilisée pour déclencher le shake")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("Paramètres par défaut")]
    [Tooltip("Force du shake (amplitude)")]
    [SerializeField] private float defaultForce = 1f;

    [Tooltip("Durée du shake en secondes (0 = utiliser la durée de l'ImpulseSource)")]
    [SerializeField] private float defaultDuration = 0.5f;

    private void Start()
    {
        Shake(2f, 0.8f);
    }

    private Coroutine _shakeCoroutine;


    public void Shake() => TriggerShake(defaultForce, defaultDuration);


    public void Shake(float force, float duration) => TriggerShake(force, duration);


    private void TriggerShake(float force, float duration)
    {
        if (impulseSource == null)
        {
            Debug.LogWarning("[CameraShake] Aucun CinemachineImpulseSource assigné !", this);
            return;
        }


        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);

        _shakeCoroutine = StartCoroutine(ShakeRoutine(force, duration));
    }

    private IEnumerator ShakeRoutine(float force, float duration)
    {
 
        impulseSource.GenerateImpulse(force);


        yield return new WaitForSeconds(duration);


        CinemachineImpulseManager.Instance.Clear();

        _shakeCoroutine = null;
    }

}