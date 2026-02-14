using UnityEngine;
using System.Collections;

public class FinishEnigme : MonoBehaviour
{
    [Header("Configurations")]
    public Transform targetPoint; // Le point B
    public float duration = 2.0f; // Temps du trajet en secondes

    // Cette fonction est celle que tu appelleras dans ton Unity Event
    public void StartMoving()
    {
        if (targetPoint != null)
        {
            StopAllCoroutines(); // Évite les conflits si on clique plusieurs fois
            StartCoroutine(LerpObject(transform.position, targetPoint.position, transform.rotation, targetPoint.rotation));
        }
    }

    private IEnumerator LerpObject(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            // Calcul du ratio de progression (entre 0 et 1)
            float t = timeElapsed / duration;

            // Interpolation de la position et de la rotation
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            timeElapsed += Time.deltaTime;
            yield return null; // Attend la frame suivante
        }

        // On s'assure d'être exactement à destination à la fin
        transform.position = endPos;
        transform.rotation = endRot;
    }
}