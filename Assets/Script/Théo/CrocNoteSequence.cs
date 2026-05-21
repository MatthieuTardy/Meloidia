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
    public float cameraSequenceDuration = 5f;

    private NavMeshAgent agent;
    private Animator animator;
    private LegumeManager legumeManager;
    private Rigidbody carriedRb;
    private Collider carriedCollider;
    private float baseSpeed;
    private float originalPropHeight;
    private bool isCarrying;
    private CinemachineVirtualCamera sequenceCam;

    private enum SequenceState { Idle, MovingToPickup, PickupPause, MovingToDrop, DropPause, Fleeing, Done }
    private SequenceState currentState = SequenceState.Idle;

    private float stateTimer = 0f;
    private float cameraTimer = 0f;

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

    private void Update()
    {
        if (sequenceCam != null)
        {
            cameraTimer -= Time.deltaTime;
            if (cameraTimer <= 0f)
            {
                Destroy(sequenceCam.gameObject);
            }
        }

        if (currentState == SequenceState.Idle || currentState == SequenceState.Done)
            return;

        if (animator != null)
        {
            if (currentState == SequenceState.Fleeing)
                animator.speed = fleeSpeedMultiplier;
            else
                animator.speed = isCarrying ? carrySpeedMultiplier : 1f;

            animator.SetBool("walk", agent.velocity.sqrMagnitude > 0.01f);
        }

        switch (currentState)
        {
            case SequenceState.MovingToPickup:
                if (HasReachedDestination())
                {
                    transform.LookAt(new Vector3(pickupPoint.position.x, transform.position.y, pickupPoint.position.z));

                    if (objectToCarry != null)
                    {
                        if (carriedRb != null) carriedRb.isKinematic = true;
                        if (carriedCollider != null) carriedCollider.enabled = false;

                        objectToCarry.transform.SetParent(transform);
                        isCarrying = true;
                    }

                    if (animator != null) animator.SetBool("walk", false);
                    onPickup?.Invoke();

                    stateTimer = 1.5f;
                    currentState = SequenceState.PickupPause;
                }
                break;

            case SequenceState.PickupPause:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    if (agent != null)
                    {
                        agent.speed = baseSpeed * carrySpeedMultiplier;
                        agent.SetDestination(dropPoint.position);
                    }
                    currentState = SequenceState.MovingToDrop;
                }
                break;

            case SequenceState.MovingToDrop:
                if (HasReachedDestination())
                {
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

                    if (animator != null) animator.SetBool("walk", false);
                    onDrop?.Invoke();

                    stateTimer = 0.5f;
                    currentState = SequenceState.DropPause;
                }
                break;

            case SequenceState.DropPause:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    if (agent != null)
                    {
                        agent.speed = baseSpeed * fleeSpeedMultiplier;
                        agent.SetDestination(fleePoint.position);
                    }
                    currentState = SequenceState.Fleeing;
                }
                break;

            case SequenceState.Fleeing:
                if (HasReachedDestination())
                {
                    if (animator != null)
                    {
                        animator.speed = 1f;
                        animator.SetBool("walk", false);
                    }

                    if (legumeManager != null)
                    {
                        legumeManager.enabled = true;
                        legumeManager.StopFollowingLocation();
                    }

                    currentState = SequenceState.Done;
                    this.enabled = false;
                }
                break;
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
                float bounce = (animator != null && animator.GetBool("walk_bras_levé")) ? Mathf.Abs(Mathf.Sin(Time.time * 15f)) * 0.15f : 0f;
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
        if (legumeManager != null)
        {
            legumeManager.StopAllCoroutines();
            legumeManager.enabled = false;
        }

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

            cameraTimer = cameraSequenceDuration;
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

        currentState = SequenceState.MovingToPickup;
    }

    private bool HasReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f;
    }
}