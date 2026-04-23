using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CrocNoteCarrySequence : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform pickupPoint;
    public Transform dropPoint;
    public Transform fleePoint;

    [Header("Carry Settings")]
    public GameObject objectToCarry;
    public Transform highestBone;
    public float carryHeightOffset = 0.5f;

    [Header("Events")]
    public UnityEvent onPickup;
    public UnityEvent onDrop;

    [Header("Settings")]
    public float carrySpeedMultiplier = 0.8f;
    public float fleeSpeedMultiplier = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private LegumeManager legumeManager;
    private Rigidbody carriedRb;
    private Collider carriedCollider;
    private float baseSpeed;
    private float originalPropHeight;
    private bool isCarrying;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        legumeManager = GetComponent<LegumeManager>();

        if (legumeManager != null)
        {
            animator = legumeManager.animator;
            baseSpeed = legumeManager.vitesse;
        }

        if (objectToCarry != null)
        {
            originalPropHeight = objectToCarry.transform.position.y;
            carriedRb = objectToCarry.GetComponent<Rigidbody>();
            carriedCollider = objectToCarry.GetComponent<Collider>();
        }
    }

    private void LateUpdate()
    {
        if (isCarrying && objectToCarry != null)
        {
            if (highestBone != null)
            {
                objectToCarry.transform.position = new Vector3(transform.position.x, highestBone.position.y + carryHeightOffset, transform.position.z);
            }
            else
            {
                float bounce = (animator != null && animator.GetBool("walk")) ? Mathf.Abs(Mathf.Sin(Time.time * 15f)) * 0.15f : 0f;
                objectToCarry.transform.localPosition = new Vector3(0f, carryHeightOffset + bounce, 0f);
            }
        }

        if (legumeManager != null && !legumeManager.enabled && legumeManager.NameBoard != null)
        {
            if (GameManager.Instance != null && GameManager.Instance.playerManager != null)
            {
                legumeManager.NameBoard.transform.LookAt(GameManager.Instance.playerManager.Camera.transform);
            }
        }
    }

    public void StartSequence()
    {
        StartCoroutine(SequenceRoutine());
    }

    private IEnumerator SequenceRoutine()
    {
        if (legumeManager != null)
        {
            legumeManager.StopAllCoroutines();
            legumeManager.enabled = false;
        }

        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = baseSpeed;
            agent.SetDestination(pickupPoint.position);
        }

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.2f)
        {
            if (animator != null)
            {
                animator.speed = 1f;
                animator.SetBool("walk", agent.velocity.sqrMagnitude > 0.01f);
            }
            yield return null;
        }

        if (animator != null) animator.SetBool("walk", false);

        transform.LookAt(new Vector3(pickupPoint.position.x, transform.position.y, pickupPoint.position.z));

        if (objectToCarry != null)
        {
            if (carriedRb != null) carriedRb.isKinematic = true;
            if (carriedCollider != null) carriedCollider.enabled = false;

            objectToCarry.transform.SetParent(transform);
            isCarrying = true;
        }

        onPickup?.Invoke();

        yield return new WaitForSeconds(0.2f);

        if (agent != null)
        {
            agent.speed = baseSpeed * carrySpeedMultiplier;
            agent.SetDestination(dropPoint.position);
        }

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.2f)
        {
            if (animator != null)
            {
                animator.speed = carrySpeedMultiplier;
                animator.SetBool("walk", agent.velocity.sqrMagnitude > 0.01f);
            }
            yield return null;
        }

        if (animator != null) animator.SetBool("walk", false);

        transform.LookAt(new Vector3(dropPoint.position.x, transform.position.y, dropPoint.position.z));

        if (objectToCarry != null)
        {
            isCarrying = false;
            objectToCarry.transform.SetParent(null);

            Vector3 finalDropPosition = objectToCarry.transform.position;
            finalDropPosition.y = originalPropHeight;
            objectToCarry.transform.position = finalDropPosition;

            if (carriedRb != null) carriedRb.isKinematic = false;
            if (carriedCollider != null) carriedCollider.enabled = true;
        }

        onDrop?.Invoke();

        yield return new WaitForSeconds(0.2f);

        if (agent != null)
        {
            agent.speed = baseSpeed * fleeSpeedMultiplier;
            agent.SetDestination(fleePoint.position);
        }

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.2f)
        {
            if (animator != null)
            {
                animator.speed = fleeSpeedMultiplier;
                animator.SetBool("walk", agent.velocity.sqrMagnitude > 0.01f);
            }
            yield return null;
        }

        if (animator != null) animator.speed = 1f;

        if (legumeManager != null)
        {
            legumeManager.enabled = true;
            legumeManager.StopFollowingLocation();
        }

        this.enabled = false;
    }
}