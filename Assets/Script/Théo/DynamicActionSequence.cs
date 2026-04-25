using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DynamicActionSequence : MonoBehaviour
{
    public Vector3 targetPosition;
    public EnigmeGroupAction manager;
    public float sequenceOffset;   // Left/Right offset
    public float sequenceOffsetZ;  // Front/Back offset (Rows)
    public float groupArrivalTolerance = 1.0f;

    private NavMeshAgent agent;
    private Animator animator;
    private LegumeManager legumeManager;
    private Rigidbody rb;
    private Collider col;
    private bool isPushing = false;

    public void StartSequence()
    {
        agent = GetComponent<NavMeshAgent>();
        legumeManager = GetComponent<LegumeManager>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (legumeManager != null)
        {
            animator = legumeManager.animator;
        }
        StartCoroutine(MoveToTarget());
    }

    private void Update()
    {
        // Make sure the NameBoard keeps looking at the camera while LegumeManager is disabled
        if (legumeManager != null && legumeManager.NameBoard != null)
        {
            if (GameManager.Instance != null && GameManager.Instance.playerManager != null && GameManager.Instance.playerManager.Camera != null)
            {
                legumeManager.NameBoard.transform.LookAt(GameManager.Instance.playerManager.Camera);
            }
        }
    }

    private IEnumerator MoveToTarget()
    {
        if (legumeManager != null)
        {
            legumeManager.StopAllCoroutines();
            legumeManager.enabled = false;
        }

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }

        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;

        if (agent != null)
        {
            while (!agent.isOnNavMesh) yield return null;

            agent.radius = 0.1f;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            agent.SetDestination(targetPosition);
        }

        yield return null;

        while (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            float dist = Vector2.Distance(
                new Vector2(transform.position.x, transform.position.z),
                new Vector2(targetPosition.x, targetPosition.z)
            );

            if (dist <= groupArrivalTolerance)
            {
                break;
            }

            if (animator != null) animator.SetBool("walk", true);
            yield return null;
        }

        if (animator != null) animator.SetBool("walk", false);

        if (agent != null)
        {
            // --- FIX: Ensure agent is active and on navmesh before stopping ---
            if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
            agent.enabled = false;
        }

        if (manager != null && manager.propToPush != null)
        {
            transform.SetParent(manager.propToPush.transform, true);

            // Calculate exact position using both X (left/right) and Z (front/back) offsets
            Vector3 exactPos = manager.targetPoint.position
                             + (manager.targetPoint.right * sequenceOffset)
                             + (manager.targetPoint.forward * sequenceOffsetZ);
            exactPos.y = manager.targetPoint.position.y;

            transform.position = exactPos;
            transform.rotation = Quaternion.LookRotation(manager.targetPoint.forward);
            transform.Rotate(0, manager.rotationOffset, 0);

            manager.OnCrocNoteArrived(this);
        }
        else if (manager != null)
        {
            manager.OnCrocNoteArrived(this);
        }
    }

    public void StartPushing()
    {
        isPushing = true;
        StartCoroutine(ForcePushAnimation());
    }

    private IEnumerator ForcePushAnimation()
    {
        while (isPushing)
        {
            if (animator != null)
            {
                animator.SetBool("walk", true);
            }
            yield return null;
        }
    }

    public void FinishAndReturn()
    {
        isPushing = false;
        StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        transform.SetParent(null);

        if (agent != null) agent.enabled = false;

        yield return null;

        if (rb != null) rb.isKinematic = false;
        if (col != null) col.enabled = true;

        if (agent != null)
        {
            agent.enabled = true;
            yield return new WaitForFixedUpdate();
        }

        if (legumeManager != null)
        {
            if (animator != null) animator.SetBool("walk", false);
            legumeManager.enabled = true;

            if (GameManager.Instance != null && GameManager.Instance.playerManager != null)
            {
                legumeManager.StartFollowingLocation(GameManager.Instance.playerManager.transform);
            }
        }
        Destroy(this);
    }
}