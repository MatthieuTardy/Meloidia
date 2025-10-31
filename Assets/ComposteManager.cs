using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComposteManager : MonoBehaviour
{
    private bool detected = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 2 && GameManager.Instance.playerManager.terre < GameManager.Instance.playerManager.terreMax)
        {
            if (GameManager.Instance.playerManager.indexTuto == 1)
            {
                GameManager.Instance.playerManager.indexTuto += 1;
                GameManager.Instance.playerManager.tutoSelect = GameManager.Instance.playerManager.tuto[GameManager.Instance.playerManager.indexTuto];
            }

            GameManager.Instance.playerManager.terre = GameManager.Instance.playerManager.terreMax;
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