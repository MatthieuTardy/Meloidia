using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WildCrocNote : MonoBehaviour
{
    [Header("Bonheur")]
    MelogumeSingingManager melogumesSingingManager;
    private GameObject baseLegume;

    [Header("Gestion de la colère")]
    [SerializeField] musicalNotes[] CalmMelody;
    bool isAttacking = false;
    bool canAttackPlayer = false;

    [Header("Paramètres de Déplacement")]
    [SerializeField] float walkRadius = 5f;
    [SerializeField] float intervalleAttente = 5f;
    public float vitesse = 5f;
    [SerializeField] float vitesseRotation = 10f;
    [SerializeField] NavMeshAgent myNavAgent;
    Transform nextPoint;
    private Coroutine move;
    private Coroutine attack;

    [Header("Effets de 'Juice' - Respiration")]
    public float amplitudeRespiration = 0.05f;
    public float vitesseRespiration = 2f;

    private Rigidbody rb;
    WildCrocNoteTriggerZone WCNTZ;

    [SerializeField] GameObject player;

    #region Unity Default Function
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;
        WCNTZ = GetComponentInChildren<WildCrocNoteTriggerZone>();
        move = StartCoroutine(RandomMove());

    }
    private void Update()
    {
 
    }
    #endregion

    #region movement
    public IEnumerator RandomMove()
    {
        yield return new WaitForSeconds(Random.Range(1, 5));
        if (!WCNTZ.isTouchingPlayer)
        {
            Debug.Log("Can't attack player");
            myNavAgent.SetDestination(RandomNavmeshLocation(walkRadius));
            move = StartCoroutine(RandomMove());
        }
        else
        {
            Debug.Log("Can atk player");
            StartAttackPlayer();
            yield return new WaitUntil(() => !canAttackPlayer);
            move = StartCoroutine(RandomMove());
        }
    }
    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    IEnumerator MoveToTarget()
    {
        myNavAgent.SetDestination(nextPoint.position);
        yield return new WaitUntil(() => Vector3.Distance(this.transform.position, nextPoint.position) <= 1f);
        StartCoroutine(MoveToTarget());
    }

    #endregion
    #region attack
    public void StartAttackPlayer()
    {




        canAttackPlayer = true;
        StopCoroutine(move);
        attack = StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        Debug.Log("Attack");
        yield return new WaitForSeconds(Random.Range(1f, 4f));
        myNavAgent.SetDestination(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));
        attack = StartCoroutine(AttackRoutine());

    }

    void EndAttack()
    {
        canAttackPlayer = false;
        nextPoint = null;
        move = StartCoroutine(RandomMove());
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            Vector3 posi = player.transform.position - transform.position;
            Debug.Log("posi" + posi);
            GameManager.Instance.playerManager.Bump(player.transform.position - transform.position);
        }
    }
    #endregion
    /*
    void StartRageState(Transform other)
    {
        StopCoroutine(move);
        transform.LookAt(other);

        StartCoroutine(RageState());
    }

    public IEnumerator RageState()
    {

        isStartRageTimer = 1;
        melogumesSingingManager.StopHappyness();
        StartCoroutine(EndOfRageDelay(deathTimer, chanceToDie));
        melogumesSingingManager.StartRage();
        colere = true;
        Debug.Log("Colère !");
        yield return new WaitUntil(() => colere == false);
        melogumesSingingManager.StopRage();
        melogumesSingingManager.StartHappyness();
        move = StartCoroutine(RandomMove());
        Debug.Log("Calme !");
        calmeTimer = 0;
        //remettre la marche
    }

    IEnumerator EndOfRageDelay(float EndOfRageTimer, int deathChance)
    {
        yield return new WaitForSeconds(EndOfRageTimer);
        int jetDeConstitution = Random.Range(0, 100);
        if (jetDeConstitution < deathChance && colere == true)
        {
            Destroy(gameObject);
        }
        else if (jetDeConstitution >= deathChance && colere == true)
        {
            colere = false;
        }
        else
        {
            colere = false;
        }

    }

    */
}
