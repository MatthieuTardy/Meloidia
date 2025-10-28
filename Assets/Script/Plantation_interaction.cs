using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Plantation_interaction : MonoBehaviour
{
    private bool detected = false;
    public GameObject plante;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        detected = false;
    }

    IEnumerator Action()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //Mettre le mesh de plante ici
            plante.SetActive(true);
            Debug.Log("Touché");
        }
        if (detected == false)
        {
            yield break;
        }
        yield return null;

    }
}
