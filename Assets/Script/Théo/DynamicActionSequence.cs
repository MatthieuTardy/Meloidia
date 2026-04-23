using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DynamicActionSequence : MonoBehaviour
{
    public Vector3 targetPosition;
    public EnigmeGroupAction manager;
    public float groupArrivalTolerance = 0.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private LegumeManager legumeManager;
    private float baseSpeed;

    public void StartSequence()
    {
        agent = GetComponent<NavMeshAgent>();
        legumeManager = GetComponent<LegumeManager>();

        if (legumeManager != null)
        {
            animator = legumeManager.animator;
            baseSpeed = legumeManager.vitesse;
        }

        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        if (legumeManager != null)
        {
            legumeManager.StopAllCoroutines();
            legumeManager.enabled = false;
        }

        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = false;
            agent.speed = baseSpeed;
            agent.acceleration = 20f;
            agent.SetDestination(targetPosition);
        }

        float timeoutTimer = 0f;
        float maxWaitTime = 7f;

        while (true)
        {
            Vector3 flatPos = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 flatTarget = new Vector3(targetPosition.x, 0f, targetPosition.z);

            if (Vector3.Distance(flatPos, flatTarget) <= (agent.stoppingDistance + groupArrivalTolerance))
            {
                break;
            }

            timeoutTimer += Time.deltaTime;
            if (timeoutTimer >= maxWaitTime) break;

            if (agent != null) agent.SetDestination(targetPosition);

            if (animator != null)
            {
                animator.speed = 1f;
                animator.SetBool("walk", agent.velocity.sqrMagnitude > 0.01f);
            }
            yield return null;
        }

        if (agent != null)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }

        if (animator != null) animator.SetBool("walk", false);

        if (manager != null)
        {
            transform.rotation = Quaternion.LookRotation(manager.targetPoint.forward);

            if (manager.propToPush != null)
            {
                transform.SetParent(manager.propToPush.transform);
            }

            manager.OnCrocNoteArrived(this);
        }
    }

    public void StartPushing()
    {
        if (animator != null)
        {
            animator.speed = 1f;
            animator.SetBool("walk", true);
        }
    }

    public void FinishAndReturn()
    {
        StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        transform.SetParent(null);

        if (animator != null) animator.SetBool("walk", false);

        if (agent != null)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 4.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }

            agent.isStopped = false;
            agent.ResetPath();
        }

        yield return null;

        if (legumeManager != null)
        {
            legumeManager.enabled = true;
            legumeManager.StartFollowingLocation(GameManager.Instance.playerManager.transform);
        }

        Destroy(this);
    }
}