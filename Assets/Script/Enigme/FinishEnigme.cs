using UnityEngine;
using System.Collections;

public class FinishEnigme : MonoBehaviour
{
    [Header("Configurations")]
    public Transform targetPoint;
    public float duration = 0.5f; 

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Coroutine currentMoveCoroutine;
    [SerializeField] ProgressEnigmeSystem ProgressEnigmeSystem;

    void Awake()
    {
        // On mémorise la position d'origine au lancement
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    public void StartMoving()
    {
        if (targetPoint != null)
        {
            StopAllCoroutines(); 
            StartCoroutine(LerpObject(transform.position, targetPoint.position, transform.rotation, targetPoint.rotation));
        }
    }

    public void ProgressMoving()
    {
        if (targetPoint != null)
        {
            StopAllCoroutines();
            MoveToStep(ProgressEnigmeSystem.ratio);
        }
    }

    public void MoveToStep(float ratio)
    {
        if (targetPoint == null) return;

        Vector3 nextPos = Vector3.Lerp(startPosition, targetPoint.position, ratio);
        Quaternion nextRot = Quaternion.Slerp(startRotation, targetPoint.rotation, ratio);

        if (currentMoveCoroutine != null) StopCoroutine(currentMoveCoroutine);
        currentMoveCoroutine = StartCoroutine(LerpObject(transform.position, nextPos, transform.rotation, nextRot));
    }

    private IEnumerator LerpObject(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {

            float t = timeElapsed / duration;


            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            timeElapsed += Time.deltaTime;
            yield return null; 
        }

        // On s'assure d'être exactement à destination à la fin
        transform.position = endPos;
        transform.rotation = endRot;
    }
}