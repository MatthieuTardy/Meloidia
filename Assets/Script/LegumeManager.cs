using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class LegumeManager : MonoBehaviour
{
    public enum type
    {
        un = 1,
        deux = 2,
        trois = 3
    }

    public int Specisme1 = 50;
    public int Specisme2;
    public int Specisme3;

    public MelogumeSingingManager melogumesSingingManager;

    public bool colere;

    public type legumeType;
    private string legumeName;
    
    
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

    [Header("Effets de 'Juice' - Saut")]
    public float hauteurSaut = 0.5f;
    [Tooltip("Intensité de l'étirement vertical pendant le saut.")]
    public float intensiteEtirement = 0.3f;
    [Tooltip("Intensité de l'écrasement horizontal pour compenser.")]
    public float intensiteEcrasement = 0.15f;


    [Header("Paramètres de Transition")]
    public float tempsTransition = 0.25f;

    private Rigidbody rb;
    private Vector3 directionAleatoire;
    private Vector3 echelleInitiale;
    private float positionYInitiale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;
        StartCoroutine(PosY());
        echelleInitiale = transform.localScale;
        

        etatActuel = Etat.Attente;
        StartCoroutine(MachineDEtats());
    }
    IEnumerator PosY()
    {
        yield return new WaitForSeconds(1);
        positionYInitiale = transform.position.y;
    }
    IEnumerator MachineDEtats()
    {
        while (true)
        {
            switch (etatActuel)
            {
                case Etat.Attente:
                    yield return StartCoroutine(EtatAttente());
                    break;
                case Etat.TransitionVersDeplacement:
                    yield return StartCoroutine(EtatTransitionVersDeplacement());
                    break;
                case Etat.Deplacement:
                    yield return StartCoroutine(EtatDeplacement());
                    break;
            }
        }
    }

    IEnumerator EtatAttente()
    {
        rb.velocity = Vector3.zero;
        float tempsAttenteEcoule = 0f;

        while (tempsAttenteEcoule < intervalleAttente)
        {
            float scaleOffset = Mathf.Sin(Time.time * vitesseRespiration) * amplitudeRespiration;
            transform.localScale = echelleInitiale + new Vector3(0, scaleOffset, 0);
            tempsAttenteEcoule += Time.deltaTime;
            yield return null;
        }

        etatActuel = Etat.TransitionVersDeplacement;
    }

    IEnumerator EtatTransitionVersDeplacement()
    {
        float tempsEcoule = 0f;
        Vector3 echelleActuelle = transform.localScale;
        directionAleatoire = new Vector3(Random.Range(-1f, 1f), rb.velocity.y, Random.Range(-1f, 1f)).normalized;
        if (directionAleatoire == Vector3.zero) directionAleatoire = Vector3.forward;

        while (tempsEcoule < tempsTransition)
        {
            transform.localScale = Vector3.Lerp(echelleActuelle, echelleInitiale, tempsEcoule / tempsTransition);
            Quaternion rotationCible = Quaternion.LookRotation(directionAleatoire);
            rb.rotation = Quaternion.Slerp(rb.rotation, rotationCible, Time.deltaTime * vitesseRotation);
            tempsEcoule += Time.deltaTime;
            yield return null;
        }

        transform.localScale = echelleInitiale;
        etatActuel = Etat.Deplacement;
    }

    IEnumerator EtatDeplacement()
    {
        float tempsEcoule = 0f;
        // *** LA CORRECTION EST ICI ***
        // On mémorise la position horizontale au début du déplacement.
        Vector3 positionHorizontaleActuelle = rb.position;

        while (tempsEcoule < dureeDeplacement)
        {
            // --- Calcul de la progression et de la courbe de saut ---
            float progression = tempsEcoule / dureeDeplacement;
            float courbeSaut = Mathf.Sin(progression * Mathf.PI);

            // --- Déplacement ---
            // 1. On met à jour notre position horizontale de référence
            positionHorizontaleActuelle += directionAleatoire * vitesse * Time.deltaTime;

            // 2. On calcule la hauteur du saut
            float deplacementVertical = courbeSaut * hauteurSaut;

            // 3. On combine la position horizontale au sol avec la hauteur du saut
            Vector3 nouvellePosition = new Vector3(
                positionHorizontaleActuelle.x,
                positionYInitiale + deplacementVertical,
                positionHorizontaleActuelle.z
            );
            rb.MovePosition(nouvellePosition);

            // --- SQUASH & STRETCH ---
            float etirement = courbeSaut * intensiteEtirement;
            float ecrasement = courbeSaut * intensiteEcrasement;

            float scaleY = echelleInitiale.y + etirement;
            float scaleX = echelleInitiale.x - ecrasement;
            float scaleZ = echelleInitiale.z - ecrasement;

            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

            // --- Rotation ---
            Quaternion rotationCible = Quaternion.LookRotation(directionAleatoire);
            rb.rotation = Quaternion.Slerp(rb.rotation, rotationCible, Time.deltaTime * vitesseRotation);

            tempsEcoule += Time.deltaTime;
            yield return null;
        }

        // Rétablir la position et l'échelle à la fin
        Vector3 posFinale = rb.position;
        posFinale.y = positionYInitiale;
        rb.MovePosition(posFinale);
        transform.localScale = echelleInitiale;

        etatActuel = Etat.Attente;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7) 
        { 
            if (other.gameObject.GetComponent<LegumeManager>().legumeType == type.un)
            {
                int jetDeHaine = Random.Range(0, 100);
                Debug.Log(jetDeHaine);
                if (jetDeHaine < Specisme1)
                {
                    StopAllCoroutines();

                    transform.LookAt(other.transform);

                    StartCoroutine(RageState());
                }
            }
            if (other.gameObject.GetComponent<LegumeManager>().colere == true)
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
        
        StopCoroutine(melogumesSingingManager.joyeux);
        StartCoroutine(DeathDelay(5, 50));
        Coroutine rage = StartCoroutine(melogumesSingingManager.SongOfRage());
        colere = true;
        Debug.Log("Colère !");

        yield return new WaitUntil(()=> colere == false);
        StopCoroutine(rage);
        StartCoroutine(melogumesSingingManager.SongOfHealing());
        StartCoroutine(MachineDEtats());
        Debug.Log("Calme !");

    }

    public IEnumerator DeathDelay(float deathTimer, int deathChance)
    {


        yield return new WaitForSeconds(deathTimer);
        int jetDeConstitution = Random.Range(0, 100);
        if (jetDeConstitution < deathChance)
        {
            Destroy(gameObject);
        }
        else
        {
            colere = false;
        }

    }
}