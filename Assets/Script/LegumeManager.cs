using System.Collections;
using UnityEngine;
using NaughtyAttributes;


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
    public int Specisme1 = 50;
    public int Specisme2 = 50;
    public int Specisme3 = 50;
    public int Specisme4 = 50;
    public int Specisme5 = 50;

    public MelogumeSingingManager melogumesSingingManager;
    private GameObject baseLegume;


    public type legumeType;
    private string legumeName;


    [Header("Gestion de la mort")]
    public float deathTimer = 15;
    public int chanceToDie = 50;

    [Header("Gestion de la colère")]
    private int jetDeHaine;
    private float calmeTimer = 30;
    public float finCalme = 30;
    public bool colere;
    public float isStartRageTimer = -1f;
    private enum Etat { Attente, TransitionVersDeplacement, Deplacement }
    private Etat etatActuel;

    [Header("Paramètres de Déplacement")]
    public float dureeDeplacement = 1f;
    public float intervalleAttente = 5f;
    public float vitesse = 5f;
    public float vitesseRotation = 10f;

    [Header("Effets de 'Juice' - Respiration")]
    public float amplitudeRespiration = 0.05f;
    public float vitesseRespiration = 2f;

    private Rigidbody rb;
    private Vector3 directionAleatoire;
    private Vector3 echelleInitiale;
    private float positionYInitiale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;
        baseLegume = FindObjectOfType<PlayerManager>();

        echelleInitiale = transform.localScale;
        Rename();



        etatActuel = Etat.Attente;

    }

    private void Update()
    {
        calmeTimer += Time.deltaTime;
        if (colere == true && isStartRageTimer >= -0.5f)
        {
            isStartRageTimer -= Time.deltaTime;
        }

    }

    private void Rename()
    {
        this.gameObject.name = NameCreator.NewName();
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
                            StopAllCoroutines();

                            transform.LookAt(other.transform);

                            StartCoroutine(RageState());
                        }
                        break;
                    case type.deux:
                        Debug.Log(jetDeHaine);
                        if (jetDeHaine < Specisme2)
                        {
                            StopAllCoroutines();

                            transform.LookAt(other.transform);

                            StartCoroutine(RageState());
                        }
                        break;
                    case type.trois:
                        Debug.Log(jetDeHaine);
                        if (jetDeHaine < Specisme3)
                        {
                            StopAllCoroutines();

                            transform.LookAt(other.transform);

                            StartCoroutine(RageState());
                        }
                        break;
                    case type.quatre:
                        Debug.Log(jetDeHaine);
                        if (jetDeHaine < Specisme4)
                        {
                            StopAllCoroutines();

                            transform.LookAt(other.transform);

                            StartCoroutine(RageState());
                        }
                        break;
                    case type.cinq:
                        Debug.Log(jetDeHaine);
                        if (jetDeHaine < Specisme5)
                        {
                            StopAllCoroutines();

                            transform.LookAt(other.transform);

                            StartCoroutine(RageState());
                        }
                        break;

                }
            }

            else if (other.gameObject.GetComponent<LegumeManager>().colere == true && other.gameObject.GetComponent<LegumeManager>().isStartRageTimer > 0f && colere == false)
            {
                StopAllCoroutines();
                StartCoroutine(RageState());
                transform.LookAt(other.transform);

            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (GameManager.Instance.playerManager.calme == true)
        {
            colere = false;
        }
    }
    public IEnumerator RageState()
    {
        isStartRageTimer = 1;
        StopCoroutine(melogumesSingingManager.joyeux);
        StartCoroutine(DeathDelay(deathTimer, chanceToDie));
        Coroutine rage = StartCoroutine(melogumesSingingManager.SongOfRage());
        colere = true;
        Debug.Log("Colère !");

        yield return new WaitUntil(()=> colere == false);
        StopCoroutine(rage);
        StartCoroutine(melogumesSingingManager.SongOfHealing());

        Debug.Log("Calme !");
        calmeTimer = 0;
    }

    public IEnumerator DeathDelay(float deathTimer, int deathChance)
    {


        yield return new WaitForSeconds(deathTimer);
        int jetDeConstitution = Random.Range(0, 100);
        if (jetDeConstitution < deathChance && colere == true)
        {
            Destroy(gameObject);
        }
        else
        {
            colere = false;
        }

    }
}