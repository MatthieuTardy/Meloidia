using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanchManager : MonoBehaviour
{
    // si des crocs Notes sont dans le ranch
    //on change leur surface de déplacement pour la zone du ranch
    // on les ajoutes dans une listes -> liste utilisé par les conditions
    // 


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 7)//legume
        {
            ChangeNavMesh(collision.GetComponent<LegumeManager>());
        }
    }


    void ChangeNavMesh(LegumeManager CN)
    {
        CN.myNavAgent.areaMask = 4;
    }
}
