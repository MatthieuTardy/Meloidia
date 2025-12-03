using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            Debug.Log("PickUp " + other.gameObject.name);
            GameManager.Instance.inventoryManager.AddItem(other.GetComponent<Ressources>());
        }
    }
}
