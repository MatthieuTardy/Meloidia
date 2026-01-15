using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using TMPro;


[RequireComponent(typeof(Rigidbody))]
public class LegumeManager : MonoBehaviour
{
    public enum type
    {
        un = 1,
        deux = 2,
        trois = 3,
        quatre = 4,
        cinq = 5
    }
    [Header("Haine par type en %")]
    public int Specisme1 = 5;
    public int Specisme2 = 50;
    public int Specisme3 = 50;
    public int Specisme4 = 50;
    public int Specisme5 = 50;

    [Header("Bonheur")]
    [Range(0, 100)] [SerializeField] int bonheur = 50;

    public MelogumeSingingManager melogumesSingingManager;
    private GameObject baseLegume;


    public type legumeType;
    private string legumeName;


    [Header("Gestion de la mort")]
    public float deathTimer = 15;
    public int chanceToDie = 5;

    [Header("Gestion de la colère")]
    public float finCalme = 30;
    public bool colere;
    private int jetDeHaine;
    private float calmeTimer = 30;
    public float isStartRageTimer = -1f;
    private enum Etat { Attente, TransitionVersDeplacement, Deplacement }
    private Etat etatActuel;

    [Header("Paramètres de Déplacement")]
    [SerializeField] float walkRadius = 5f;
    [SerializeField] float intervalleAttente = 5f;
    public float vitesse = 5f;
    [SerializeField] float vitesseRotation = 10f;
    public NavMeshAgent myNavAgent;
    private Coroutine move;

    [Header("Effets de 'Juice' - Respiration")]
    public float amplitudeRespiration = 0.05f;
    public float vitesseRespiration = 2f;

    private Rigidbody rb;
    public GameObject NameBoard;


    void Start()
    {
        GameManager.Instance.AddCrocNote(this);
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;
        baseLegume = FindObjectOfType<PlayerManager>().gameObject;
        Rename();

        etatActuel = Etat.Attente;
        move = StartCoroutine(RandomMove());

    }


    public IEnumerator RandomMove()
    {
        yield return new WaitForSeconds(Random.Range(1, 5));
        myNavAgent.SetDestination(RandomNavmeshLocation(walkRadius));
        move = StartCoroutine(RandomMove());

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
        NameBoard.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
    }

    private void Rename()
    {
        this.gameObject.name = NameCreator.NewName();
        NameBoard.GetComponent<TextMeshPro>().text = this.gameObject.name;
    }
 


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7 && other.GetComponent<LegumeManager>().calmeTimer >= other.GetComponent<LegumeManager>().finCalme) 
        {
            jetDeHaine = Random.Range(0, 100);
            if (other.gameObject.GetComponent<LegumeManager>().colere == false && colere == false)
            {
                switch (other.gameObject.GetComponent<LegumeManager>().legumeType)
                {
                    case type.un:
                        Debug.Log(jetDeHaine);
                        if (jetDeHaine < Specisme1)
                        {
                            StartRageState(other.transform);
                        }
                        break;
                    case type.deux:
                        Debug.Log(jetDeHaine);
                        if (jetDeHaine < Specisme2)
                        {
                            StartRageState(other.transform);
                        }
                        break;
                    case type.trois:
                        Debug.Log(jetDeHaine);
                        if (jetDeHaine < Specisme3)
                        {
                            StartRageState(other.transform);
                        }
                        break;
                    case type.quatre:
                        Debug.Log(jetDeHaine);
                        if (jetDeHaine < Specisme4)
                        {
                            StartRageState(other.transform);
                        }
                        break;
                    case type.cinq:
                        Debug.Log(jetDeHaine);
                        if (jetDeHaine < Specisme5)
                        {
                            StartRageState(other.transform);
                        }
                        break;

                }
            }

            else if (other.gameObject.GetComponent<LegumeManager>().colere == true && other.gameObject.GetComponent<LegumeManager>().isStartRageTimer > 0f && colere == false)
            {
                StartRageState(other.transform);

            }
        }
    }

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
        StartCoroutine(EndOfRageDelay(deathTimer, chanceToDie));
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
            SetBonheur(GetBonheur() - 5);
            colere = false;
        }
        else
        {
            colere = false;
        }

    }
}