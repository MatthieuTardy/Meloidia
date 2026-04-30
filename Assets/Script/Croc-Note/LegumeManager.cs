using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public enum CrocNoteType
{
    un = 1,
    deux = 2,
    trois = 3,
    quatre = 4,
    cinq = 5
}
[RequireComponent(typeof(Rigidbody))]
public class LegumeManager : MonoBehaviour
{
    #region Data
    [Header("Haine par type en %")]
    public int Specisme1 = 5;
    public int Specisme2 = 50;
    public int Specisme3 = 50;
    public int Specisme4 = 50;
    public int Specisme5 = 50;

    [Header("Bonheur")]
    [Range(0, 100)] [SerializeField] int bonheur = 50;
    [InfoBox("A partir de combien de bonheur ce CN est considerer comme triste ou heureux")]
    [Range(0, 100)] [SerializeField] int SadPercent, HappyPercent;
    public MelogumeSingingManager melogumesSingingManager;
    private GameObject baseLegume;
   

    public CrocNoteType legumeType;
    private string legumeName;


    [Header("Gestion de la mort")]
    public float deathTimer = 15;


    [Header("Gestion de la colère")]
    public float finCalme = 30;
    public bool colere;
    private int jetDeHaine;
    private float calmeTimer = 30;
    public float isStartRageTimer = -1f;
    private enum Etat { Attente, TransitionVersDeplacement, Deplacement }
    private Etat etatActuel;

    [Header("Paramètres de Déplacement")]
    public bool CanMoveFreely = true;
    private Vector3 finalPos;
    public Transform CurrentTarget;
    [SerializeField] float walkRadius = 5f;
    [SerializeField] float intervalleAttente = 5f;
    public float vitesse = 5f;
    [SerializeField] float vitesseRotation = 10f;
    public NavMeshAgent myNavAgent;
    private Coroutine move;

    ///Théo
    private float lastPathCheckTime;
    private float pathCheckInterval = 0.5f;
    ///Théo

    [Header("Effets de 'Juice' - Respiration")]
    public float amplitudeRespiration = 0.05f;
    public float vitesseRespiration = 2f;

    private Rigidbody rb;
    public GameObject NameBoard;

    [Header("Animation")]
    public Animator animator;


    #endregion
    private void Rename()
    {
        this.gameObject.name = NameCreator.NewName();
        NameBoard.GetComponent<TextMeshPro>().text = this.gameObject.name;
    }
    #region Unity default function

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.isKinematic = true;          
        rb.useGravity = false;
        baseLegume = FindObjectOfType<PlayerManager>().gameObject;
        Rename();
        GameManager.Instance.AddCrocNote(this);

        etatActuel = Etat.Attente;
        move = StartCoroutine(RandomMove());

    }

    private void Update()
    {
        calmeTimer += Time.deltaTime;
        if (colere == true && isStartRageTimer >= -0.5f)
        {
            isStartRageTimer -= Time.deltaTime;
        }
        if (calmeTimer <= finCalme)
        {
            calmeTimer += Time.deltaTime;
        }
        if (CurrentTarget == null)
        {
            if (!colere)
            {

                if (bonheur <= SadPercent)
                {
                    melogumesSingingManager.StartSadness();
                }
                else if (bonheur >= HappyPercent)
                {
                    melogumesSingingManager.StartHappyness();
                }
                else
                {
                    melogumesSingingManager.StartNormal();
                }
            }
            else
            {
                melogumesSingingManager.StartRage();
            }
        }
        if (!CanMoveFreely && CurrentTarget != null)
        {
            ///Modif théo
            float distanceToTarget = Vector3.Distance(transform.position, CurrentTarget.position);

            if (distanceToTarget <= 20f)
            {
                pathCheckInterval = 0.5f;
            }
            else
            {
                pathCheckInterval = Mathf.Clamp((distanceToTarget - 20f) * 0.5f, 1f, 25f);
            }

            bool reachedDestination = !myNavAgent.pathPending && myNavAgent.remainingDistance <= 0.5f;

            if (Time.time >= lastPathCheckTime + pathCheckInterval || reachedDestination)
            {
                lastPathCheckTime = Time.time;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(CurrentTarget.position, out hit, 100f, NavMesh.AllAreas))
                {
                    if (Vector3.Distance(myNavAgent.destination, hit.position) > 1.0f)
                    {
                        myNavAgent.SetDestination(hit.position);
                    }
                }
            }

            if (Vector3.Distance(this.transform.position, CurrentTarget.position) <= 2.1f || reachedDestination)
            {
                animator.SetBool("walk", false);
            }
            else
            {
                animator.SetBool("walk", true);
            }
            ///Modif théo
        }
        NameBoard.transform.LookAt(GameManager.Instance.playerManager.Camera); 
            //= Quaternion.Euler(new Vector3(0,0,0));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7 ) //legume
        {
            if (other.GetComponent<LegumeManager>().calmeTimer >= other.GetComponent<LegumeManager>().finCalme)
            {
                jetDeHaine = Random.Range(0, 100);
                if (other.gameObject.GetComponent<LegumeManager>().colere == false && colere == false)
                {
                    switch (other.gameObject.GetComponent<LegumeManager>().legumeType)
                    {
                        case CrocNoteType.un:
                            Debug.Log(jetDeHaine);
                            if (jetDeHaine < Specisme1)
                            {
                                StartRageState(other.transform);
                            }
                            break;
                        case CrocNoteType.deux:
                            Debug.Log(jetDeHaine);
                            if (jetDeHaine < Specisme2)
                            {
                                StartRageState(other.transform);
                            }
                            break;
                        case CrocNoteType.trois:
                            Debug.Log(jetDeHaine);
                            if (jetDeHaine < Specisme3)
                            {
                                StartRageState(other.transform);
                            }
                            break;
                        case CrocNoteType.quatre:
                            Debug.Log(jetDeHaine);
                            if (jetDeHaine < Specisme4)
                            {
                                StartRageState(other.transform);
                            }
                            break;
                        case CrocNoteType.cinq:
                            Debug.Log(jetDeHaine);
                            if (jetDeHaine < Specisme5)
                            {
                                StartRageState(other.transform);
                            }
                            break;
                    }

                    
                }
            } //rencontre un autre croc-note - jet de haine

            else if (other.gameObject.GetComponent<LegumeManager>().colere == true && other.gameObject.GetComponent<LegumeManager>().isStartRageTimer > 0f && colere == false)
            {
                StartRageState(other.transform);

            } //rejoind la baston si possible
        }

    }
    #endregion

    #region Movement
    public IEnumerator RandomMove()
    {
        //Debug.Log("move routine " + CanMoveFreely);
        if (CanMoveFreely)
        {
            yield return new WaitForSeconds(Random.Range(1, 5));
            animator.SetBool("walk", true);
            finalPos = RandomNavmeshLocation(walkRadius);

            ///Théo
            if (finalPos != Vector3.zero)
            {
                myNavAgent.SetDestination(finalPos);
                yield return new WaitUntil(() => Vector3.Distance(this.transform.position, finalPos) <= 2.1f);
            }
            ///Théo

            animator.SetBool("walk", false);
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
            ///Théo
            NavMeshPath path = new NavMeshPath();
            if (myNavAgent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                finalPosition = hit.position;
            }
            ///Théo
        }
        return finalPosition;
    }

    public void StartFollowingLocation(Transform newLoc)
    {
        CanMoveFreely = false;
        animator.SetBool("walk", true);
        if (move != null)
        {
            StopCoroutine(move);
        }
        CurrentTarget = newLoc;

    }
    public void StopFollowingLocation()
    {
        CanMoveFreely = true;
        CurrentTarget = null;
        move = StartCoroutine(RandomMove());
    }

    #endregion

    #region rage
    void StartRageState(Transform other)
    {
        StopCoroutine(move);
        transform.LookAt(other);

        StartCoroutine(RageState());
    }

    private void OnTriggerStay(Collider other)
    {
        if (GameManager.Instance.playerManager.calme == true && other.tag == "Chant")
        {
            colere = false;
        }
    }
    public IEnumerator RageState()
    {
        isStartRageTimer = 1;
        melogumesSingingManager.StopHappyness();
        StartCoroutine(EndOfRageDelay(deathTimer));
        melogumesSingingManager.StartRage();
        colere = true;
        Debug.Log("Colère !");
        yield return new WaitUntil(()=> colere == false);
        melogumesSingingManager.StopRage();
        melogumesSingingManager.StartHappyness();
        move = StartCoroutine(RandomMove());
        Debug.Log("Calme !");
        calmeTimer = 0;
        //remettre la marche
    }
    IEnumerator EndOfRageDelay(float EndOfRageTimer)
    {
        yield return new WaitForSeconds(EndOfRageTimer);

            SetBonheur(GetBonheur() - 5);
            colere = false;
    }
    #endregion
    #region BonheurVariation
    public void SetBonheur(int newBonheur)
    {
        bonheur = newBonheur;
        if (bonheur < 0) { bonheur = 0; }
        else if (bonheur > 100) { bonheur = 100; }
    }
    public int GetBonheur()
    {
        return bonheur;
    }

    public void Bonheur(int newBonheur)
    {
        bonheur = newBonheur;

    }

    #endregion BonheurVariation

}