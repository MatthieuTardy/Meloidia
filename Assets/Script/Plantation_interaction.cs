using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Plantation_interaction : MonoBehaviour
{
    private bool detected = false;
    private bool planted = false;
    private bool finished = false;
    public GameObject plante;
    public Seed graine;
    public ParticleSystem plantingParticles; // La particule à jouer
    private float timer;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (planted && timer / graine.GrowthTimeTotal >= 0.5 && timer / graine.GrowthTimeTotal < 1)
        {
            plante.transform.localPosition = new Vector3(0, 0.4f, 0);
            plante.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        }
        if (planted && timer / graine.GrowthTimeTotal >= 1)
        {
            plante.transform.localPosition = new Vector3(0, 0.45f, 0);
            plante.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
            finished = true;
        }




    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("T'es dedans");
        if (other.CompareTag("Detecteur"))
        {
            detected = true;
            Debug.Log("Peut me faire des choses");
            StartCoroutine(Action());


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Detecteur"))
        {
            detected = false;
        }
    }
    IEnumerator Grow()
    {

        yield return null;
    }

    IEnumerator Action()
    {
        if (Input.GetButtonDown("Fire1") && finished == false && planted == false)
        {
            //Mettre le mesh de plante ici
            plante.transform.localPosition = new Vector3(0, 0.35f, 0);
            plante.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            plante.SetActive(true);
            timer = 0;
            planted = true;

            //Mettre particule de plantage
            if (plantingParticles != null)
            {
                // On instancie la particule avec une rotation de -90 degrés sur l'axe X pour la rendre horizontale
                Quaternion particleRotation = Quaternion.Euler(-90, 0, 0);
                Vector3 particlePosition = plante.transform.position + new Vector3(0, 1f, 0);

                Instantiate(plantingParticles, particlePosition, particleRotation);
            }
            Debug.Log("Planté");
        }
        if (Input.GetButtonDown("Fire1") && finished == true)
        {
            finished = false;
            plante.SetActive(false);
            timer = 0;
            planted = false;
            Debug.Log("Mangé");
        }
        if (detected == false)
        {
            yield break;
        }
        yield return null;
        StartCoroutine(Action());
    }
}