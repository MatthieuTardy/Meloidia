using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProximity : MonoBehaviour
{
    public bool Proximity;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            Proximity = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            Proximity = false;
        }
    }
}


