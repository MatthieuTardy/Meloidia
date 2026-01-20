using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildCrocNoteTriggerZone : MonoBehaviour
{
    public GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            target = other.gameObject;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            target = null;
        }
    }

    public bool IsPlayerInDistance(Transform origin,float distance)
    {
        if(Vector3.Distance(origin.position,target.transform.position) <= distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
