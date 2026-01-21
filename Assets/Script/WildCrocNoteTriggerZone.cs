using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildCrocNoteTriggerZone : MonoBehaviour
{
    public bool isTouchingPlayer;
    public GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            isTouchingPlayer = true;
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
            isTouchingPlayer = false;
            target = null;
        }
    }
    
    public bool IsPlayerInDistance(Transform origin,float distance)
    {
        if(target != null)
        {
            if (Vector3.Distance(origin.position, target.transform.position) <= distance)
            {
                return true;
            }
        }
        return false;
    }
    
}
