using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    GameObject interractableObject;


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
            if (interractableObject.GetComponent<Reserve>())
            {
                interractableObject.GetComponent<Reserve>().CloseReserve();
            }

            interractableObject = null;
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10) //batiment
        {
            interractableObject = other.gameObject;
        }
        else if (other.gameObject.layer == 11)// interractable
        {
            interractableObject = other.gameObject;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1")) 
        {
            if (interractableObject != null)
            {
                if (interractableObject.GetComponent<Reserve>())
                {
                    if (interractableObject.GetComponent<Reserve>().StockingInReserve == null)
                    {
                        interractableObject.GetComponent<Reserve>().OpenReserve();
                    }
                }

                else if (interractableObject.GetComponent<Plantation_interaction>())
                {
                    interractableObject.GetComponent<Plantation_interaction>().Interract(GameManager.Instance.playerManager.outils);
                }

                else if (interractableObject.GetComponent<RessourcesRare>())
                {
                    interractableObject.GetComponent<RessourcesRare>().Interract();
                }
            
                else if (interractableObject.GetComponent<DialogueSystem>())
                {
                    interractableObject.GetComponent <DialogueSystem>().Interract();
                }
            }
        }
    }
}
