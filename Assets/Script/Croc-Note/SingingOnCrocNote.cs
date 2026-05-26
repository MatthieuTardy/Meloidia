using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingingOnCrocNote : MonoBehaviour
{
    public List<musicalNotes> chantDuFollow = new List<musicalNotes> { musicalNotes.Do, musicalNotes.Mi, musicalNotes.Sol };
    public List<musicalNotes> chantDuUnfollow = new List<musicalNotes> { musicalNotes.Sol, musicalNotes.Mi, musicalNotes.Do };
    LegumeManager Lmanager;
    bool CanFollow;

    ///Théo
    public float detectionRadius = 5f;
    public float teleportDistance = 60f;
    public float getoutAnimDuration = 1.5f;
    private Transform wanderTarget;
    private Coroutine wanderRoutine;
    private bool isPlayingGetOutAnim = false;
    ///Théo

    private void Start()
    {
        Lmanager = GetComponent<LegumeManager>();

        ///Théo
        teleportDistance = 100f;
        GameObject targetObj = new GameObject("WanderTarget_" + gameObject.name);
        wanderTarget = targetObj.transform;
        ///Théo
    }

    ///Théo
    private void Update()
    {
        if (CanFollow && !isPlayingGetOutAnim)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, GameManager.Instance.playerManager.transform.position);

            if (distanceToPlayer > teleportDistance)
            {
                UnityEngine.AI.NavMeshHit hit;
                Vector3 tpPos = GameManager.Instance.playerManager.transform.position + (Random.insideUnitSphere * 2f);
                if (UnityEngine.AI.NavMesh.SamplePosition(tpPos, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    StartCoroutine(TriggerGetOutAnimation(hit.position));
                    return;
                }
            }

            if (distanceToPlayer <= detectionRadius)
            {
                if (Lmanager.CurrentTarget != wanderTarget)
                {
                    Lmanager.StartFollowingLocation(wanderTarget);
                    if (wanderRoutine == null)
                    {
                        wanderRoutine = StartCoroutine(MoveTargetAroundPlayer());
                    }
                }
            }
            else
            {
                if (Lmanager.CurrentTarget != GameManager.Instance.playerManager.transform)
                {
                    if (wanderRoutine != null)
                    {
                        StopCoroutine(wanderRoutine);
                        wanderRoutine = null;
                    }
                    Lmanager.StartFollowingLocation(GameManager.Instance.playerManager.transform);
                }
            }
        }
    }

    private IEnumerator MoveTargetAroundPlayer()
    {
        while (CanFollow)
        {
            Vector3 randomDirection = Random.insideUnitSphere * detectionRadius;
            randomDirection.y = 0;
            wanderTarget.position = GameManager.Instance.playerManager.transform.position + randomDirection;

            yield return new WaitForSeconds(Random.Range(2f, 4f));
        }
    }

    private IEnumerator TriggerGetOutAnimation(Vector3 warpPosition)
    {
        isPlayingGetOutAnim = true;

        Transform tempTarget = Lmanager.CurrentTarget;
        Lmanager.CurrentTarget = null;

        if (Lmanager.myNavAgent.isOnNavMesh)
        {
            Lmanager.myNavAgent.isStopped = true;
            Lmanager.myNavAgent.velocity = Vector3.zero;
        }

        Lmanager.animator.SetBool("walk", false);
        Lmanager.animator.SetBool("getout", true);

        Lmanager.animator.Play("getout", 0, 0f);

        Lmanager.animator.speed = 0f;

        Lmanager.myNavAgent.Warp(warpPosition);

        Vector3 lookAtPos = GameManager.Instance.playerManager.transform.position;
        lookAtPos.y = transform.position.y;
        transform.LookAt(lookAtPos);

        yield return null;

        Lmanager.animator.speed = 1f;

        yield return new WaitForSeconds(getoutAnimDuration);

        Lmanager.animator.SetBool("getout", false);
        Lmanager.CurrentTarget = tempTarget;

        if (Lmanager.myNavAgent.isOnNavMesh)
        {
            Lmanager.myNavAgent.isStopped = false;
        }

        isPlayingGetOutAnim = false;
    }

    private void OnDestroy()
    {
        if (wanderTarget != null)
        {
            Destroy(wanderTarget.gameObject);
        }
    }
    ///Théo

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 0)
        {
            if (other.gameObject.tag == "Chant")
            {
                CheckNote();
            }
        }
    }

    void CheckNote()
    {
        if (GameManager.Instance.playerManager.noteSystem.PlayerSingCorrectPattern(chantDuFollow) && !CanFollow)
        {
            CanFollow = true;
            ///Théo
            Lmanager.StartFollowingLocation(GameManager.Instance.playerManager.transform);
            ///Théo
        }
        else if (GameManager.Instance.playerManager.noteSystem.PlayerSingCorrectPattern(chantDuUnfollow) && CanFollow)
        {
            CanFollow = false;
            ///Théo
            if (wanderRoutine != null)
            {
                StopCoroutine(wanderRoutine);
                wanderRoutine = null;
            }
            ///Théo
            Lmanager.StopFollowingLocation();
        }
    }
}