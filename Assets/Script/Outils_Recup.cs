using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outils_Recup : MonoBehaviour
{
    // Start is called before the first frame update
    private bool detected = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.havingTools == false)
        {
            GameManager.Instance.playerManager.indexTuto += 1;
            GameManager.Instance.playerManager.tutoSelect = GameManager.Instance.playerManager.tuto[GameManager.Instance.playerManager.indexTuto];
            GameManager.Instance.playerManager.havingTools = true;
            GameManager.Instance.playerManager.Gant.SetActive(true);
            Debug.Log("Outils");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Detecteur")) { detected = true; }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Detecteur")) { detected = false; }
    }
}
