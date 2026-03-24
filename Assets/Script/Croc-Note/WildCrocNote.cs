using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WildCrocNote : MonoBehaviour
{
    [Header("Bonheur")]
    MelogumeSingingManager melogumesSingingManager; 

    [Header("Gestion de la colère")]
    bool canAttackPlayer = false;

    [Header("Paramètres de Déplacement")]
    [SerializeField] float walkRadius = 5f;
    [SerializeField] float originSpeed = 5f;
    private float vitesse;
    [SerializeField] NavMeshAgent myNavAgent;
    private Coroutine currentRoutine; 

    [SerializeField] GameObject player;

    [Header("Paramètres de Combat")]
    [Tooltip("La puissance avec laquelle le joueur est repoussé.")]
    [SerializeField] float knockbackForce = 10f; // <--- NOUVELLE VARIABLE ICI

    [Header("Animation")]
    public Animator animator;
    WildCrocNoteTriggerZone WCNTZ;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
        WCNTZ = GetComponentInChildren<WildCrocNoteTriggerZone>();
        vitesse = originSpeed;
        
        currentRoutine = StartCoroutine(RandomMoveLogic());
    }

    // --- LOGIQUE DE MOUVEMENT ALEATOIRE ---
    public IEnumerator RandomMoveLogic()
    {
        while (true)
        {
            if (WCNTZ.isTouchingPlayer)
            {
                SwitchToAttack();
                yield break; 
            }

            animator.SetBool("walk", false);
            yield return new WaitForSeconds(Random.Range(1, 4));

            if (WCNTZ.isTouchingPlayer) { SwitchToAttack(); yield break; }

            Vector3 target = RandomNavmeshLocation(walkRadius);
            myNavAgent.SetDestination(target);
            animator.SetBool("walk", true);

            while (myNavAgent.remainingDistance > myNavAgent.stoppingDistance)
            {
                if (WCNTZ.isTouchingPlayer) { SwitchToAttack(); yield break; }
                yield return null; 
            }
        }
    }

    // --- LOGIQUE D'ATTAQUE ---
    public void SwitchToAttack()
    {
        if(currentRoutine != null) StopCoroutine(currentRoutine);
        canAttackPlayer = true;
        currentRoutine = StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        vitesse = originSpeed * 2;
        myNavAgent.speed = vitesse;
        
        float attackDistance = 2.0f; 

        while (canAttackPlayer)
        {
            animator.SetBool("attack", false);
            animator.SetBool("walk", true);
            myNavAgent.isStopped = false; 

            while (Vector3.Distance(transform.position, player.transform.position) > attackDistance)
            {
                myNavAgent.SetDestination(player.transform.position);
                yield return null; 
            }

            Debug.Log("Attaque lancée !");
            
            animator.SetBool("attack", true);
            animator.SetBool("walk", false); 

            float slideTimer = 0.2f;
            while(slideTimer > 0)
            {
                myNavAgent.SetDestination(player.transform.position);
                slideTimer -= Time.deltaTime;
                yield return null;
            }

            myNavAgent.ResetPath(); 
            myNavAgent.velocity = Vector3.zero; 

            yield return new WaitForSeconds(0.8f);

            animator.SetBool("attack", false);

            yield return new WaitForSeconds(0.2f);
        }

        currentRoutine = StartCoroutine(RandomMoveLogic());
    }

    // --- OUTILS ---
    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            return hit.position;
        }
        return transform.position;
    }

    // Garder ton bump physique
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 8) // Layer Player
        {
            if(canAttackPlayer) 
            {
                // Calcul de la direction : Du monstre vers le joueur
                Vector3 direction = (player.transform.position - transform.position).normalized;
                
                // On multiplie la direction par la force
                // IMPORTANT : Il faut que ta méthode Bump dans PlayerManager accepte ce Vector3 plus grand
                // Si ton PlayerManager normalise le vecteur, ça ne changera rien. 
                // Dans ce cas, il faut modifier PlayerManager pour prendre un float force en paramètre.
                
                GameManager.Instance.playerManager.Bump(direction * knockbackForce); 
            }
        }
    }
}