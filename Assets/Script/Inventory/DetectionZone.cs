using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    GameObject interractableObject;
    public List<Item> collectableObjects;

    private void Start()
    {
        collectableObjects = new List<Item>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9) //collectable
        {
            //Debug.Log("PickUp " + other.gameObject.name);
            //GameManager.Instance.inventoryManager.TryToPickUp(other.GetComponent<Item>());
            collectableObjects.Add(other.GetComponent<Item>());
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
        else if(other.gameObject.layer == 11)
        {
            interractableObject = null;
        }
        else if(other.gameObject.layer == 9)
        {
            collectableObjects.Remove(other.GetComponent<Item>());
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
        if (Input.GetButtonDown("Interract"))
        {
            // Debug.Log(interractableObject.name);
            if (interractableObject != null)
            {
                if (interractableObject.GetComponent<Reserve>())
                {
                    if (interractableObject.GetComponent<Reserve>().StockingInReserve == null)
                    {
                        interractableObject.GetComponent<Reserve>().OpenReserve();
                    }
                }
                /*
                else if (interractableObject.GetComponent<RessourcesRare>())
                {
                    interractableObject.GetComponent<RessourcesRare>().Interract();
                }
                */
                /*
                else if (interractableObject.GetComponent<DialogueSystem>())
                {
                    interractableObject.GetComponent<DialogueSystem>().Interract();
                }*/
                /*
                else if (interractableObject.GetComponent<Build_Selection>())
                {
                    interractableObject.GetComponent<Build_Selection>().Interract();
                }
                */
                /*
                else if (interractableObject.GetComponent<OnlyInterraction>())
                {
                    Debug.Log("Interract");
                    interractableObject.GetComponent <OnlyInterraction>().Interract();
                }
                */
                else if (interractableObject.GetComponent<Interractable>())
                {
                    interractableObject.GetComponent<Interractable>().Interract();
                }
            }

            //if (Input.GetButtonDown("Interract"))
            
                if (collectableObjects.Count > 0)
                {
                    bool success = GameManager.Instance.inventoryManager.TryToPickUp(collectableObjects[0]);
                    if (success)
                    {
                        collectableObjects.RemoveAt(0);
                    }
                }
            
        }
    }
}
