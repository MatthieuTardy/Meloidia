using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Cinemachine;

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

    [Header("Cinematic Camera")]
    public bool enableCinematicCamera = true;
    public Transform cinematicCameraPoint;

    private NavMeshAgent agent;
    private Animator animator;
    private LegumeManager legumeManager;
    private Rigidbody carriedRb;
    private Collider carriedCollider;
    private float baseSpeed;
    private float originalPropHeight;
    private bool isCarrying;

    private CinemachineVirtualCamera sequenceCam;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        legumeManager = GetComponent<LegumeManager>();

        if (legumeManager != null)
        {
            animator = legumeManager.animator;
            baseSpeed = legumeManager.vitesse;
        }

        if (baseSpeed <= 0.1f)
        {
            baseSpeed = 5f;
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

        // --- CINEMACHINE SETUP ---
        if (enableCinematicCamera && cinematicCameraPoint != null)
        {
            GameObject camObj = new GameObject("CrocNote_CinematicCam");
            camObj.transform.position = cinematicCameraPoint.position;

            sequenceCam = camObj.AddComponent<CinemachineVirtualCamera>();

            sequenceCam.Follow = cinematicCameraPoint;
            sequenceCam.LookAt = this.transform;
            sequenceCam.Priority = 100;

            var transposer = sequenceCam.AddCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = Vector3.zero;

            var composer = sequenceCam.AddCinemachineComponent<CinemachineComposer>();
            composer.m_TrackedObjectOffset = new Vector3(0, 1f, 0);
        }

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;

            agent.acceleration = 8f;
            agent.angularSpeed = 120f;
            agent.speed = baseSpeed;

            agent.SetDestination(pickupPoint.position);
        }

        yield return StartCoroutine(MoveToDestinationLoop("Pickup"));

        transform.LookAt(new Vector3(pickupPoint.position.x, transform.position.y, pickupPoint.position.z));

        if (objectToCarry != null)
        {
            if (carriedRb != null) carriedRb.isKinematic = true;
            if (carriedCollider != null) carriedCollider.enabled = false;

            objectToCarry.transform.SetParent(transform);
            isCarrying = true;
        }

        onPickup?.Invoke();

        // Pause so the player can watch the CrocNote hold the object
        yield return new WaitForSeconds(1.5f);

        // --- CAMERA GOES BACK TO PLAYER ---
        // By destroying the camera here, Cinemachine automatically glides back to the player
        if (sequenceCam != null)
        {
            Destroy(sequenceCam.gameObject);
        }

        if (agent != null)
        {
            agent.speed = baseSpeed * carrySpeedMultiplier;
            agent.SetDestination(dropPoint.position);
        }

        yield return StartCoroutine(MoveToDestinationLoop("DropPoint"));

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
        yield return new WaitForSeconds(0.5f);

        if (agent != null)
        {
            agent.speed = baseSpeed * fleeSpeedMultiplier;
            agent.SetDestination(fleePoint.position);
        }

        yield return StartCoroutine(MoveToDestinationLoop("FleePoint"));

        if (animator != null) animator.speed = 1f;

        if (legumeManager != null)
        {
            legumeManager.enabled = true;
            legumeManager.StopFollowingLocation();
        }

        this.enabled = false;
    }

    private IEnumerator MoveToDestinationLoop(string destinationName)
    {
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.2f)
        {
            if (animator != null)
            {
                animator.speed = (destinationName == "FleePoint") ? fleeSpeedMultiplier : (isCarrying ? carrySpeedMultiplier : 1f);
                animator.SetBool("walk", agent.velocity.sqrMagnitude > 0.01f);
            }
            yield return null;
        }

        if (animator != null) animator.SetBool("walk", false);
    }
}