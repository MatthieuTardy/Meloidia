using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9) //collectable
        {
            //Debug.Log("PickUp " + other.gameObject.name);
            GameManager.Instance.inventoryManager.TryToPickUp(other.GetComponent<Item>());

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10) //batiment
        {
                if (other.GetComponent<Reserve>())
                {
                    other.GetComponent<Reserve>().CloseReserve();
                }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10) //batiment
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (other.GetComponent<Reserve>() && other.GetComponent<Reserve>().StockingInReserve == null)
                {
                    other.GetComponent<Reserve>().OpenReserve();
                }
            }
        }
    }
}
