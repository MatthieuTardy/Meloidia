using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build_Selection : Interractable
{
    [SerializeField] GameObject BuildWheel;
    [SerializeField] ConstructionScriptable[] construction;
    [SerializeField] Transform BuildSpawn;

    [SerializeField] Transform ItemDropSpawn;

    bool BuildAlreadySelected = false;
    ConstructionScriptable selectedConstruction;
    GameObject clone;

    public override void Interract()
    {
        ShowWheel();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            HideWheel();
        }
    }
    void HideWheel()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        BuildWheel.SetActive(false);
    }
    void ShowWheel()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        BuildWheel.SetActive(true);
    }

    public void ConstructChoosen(int index)
    {
        Debug.Log("index : " + index);

        if (construction[index] != null)
        {
            if (!BuildAlreadySelected && HaveItemInInventory(construction[index]))
            {
                clone = Instantiate(construction[index].MeshPrefab, BuildSpawn.position, BuildSpawn.rotation, BuildSpawn);
                BuildAlreadySelected = true;
                selectedConstruction = construction[index];
                RemoveItemFromInventory(construction[index]);
            }
        }
        else
        {
            if (index == 4)
            {
                DeleteCurrentBuild(selectedConstruction);
                selectedConstruction = null;
            }
        }
        HideWheel();
    }



    void DeleteCurrentBuild(ConstructionScriptable construction)
    {
        Debug.Log("deleting");
        BuildAlreadySelected = false;
        Destroy(clone);
        foreach (var item in construction.RessourceNeeded)
        {
            for (int i = 0; i < item.amount; i++)
            {
                Instantiate(item.RessourcePrefab, ItemDropSpawn.position, ItemDropSpawn.rotation);
            }
        }
    }

    bool HaveItemInInventory(ConstructionScriptable construction)
    {
        bool HaveItem = false;

        int quantity = 0;
        if (GameManager.Instance.inventoryManager != null && GameManager.Instance.inventoryManager.Items.Count > 0 && construction != null)
        {
            // Debug.Log("pass the if");
            foreach (var ressources in construction.RessourceNeeded)
            {
                foreach (var item in GameManager.Instance.inventoryManager.Items)
                {
                    if (item != null)
                    {
                        if (ressources.type == item.CurrentItem.type)
                        {
                            // Debug.Log("HaveItemInInventory");
                            if (item.CurrentQuantity >= ressources.amount)
                            {
                                // Debug.Log("HaveQuantityNeeded");
                                return true;
                            }
                            else
                            {
                                quantity += item.CurrentQuantity;
                                if (quantity >= ressources.amount)
                                {
                                    Debug.Log("Have ressouces :" + HaveItem);
                                    HaveItem = true;
                                }
                                else
                                {
                                    Debug.Log("Have ressouces :" + HaveItem);
                                    HaveItem = false;
                                }
                            }
                        }
                    }
                }
            }
        }
        // Debug.LogWarning("il a pas les items");
        return HaveItem;
    }

    void RemoveItemFromInventory(ConstructionScriptable construction) // search on one slot
    {
        if (GameManager.Instance.inventoryManager != null && GameManager.Instance.inventoryManager.Items.Count > 0 && construction != null)
        {
            // Debug.LogWarning("Pass in RemoveItemFromInventory");
            foreach (var ressources in construction.RessourceNeeded)
            {

                int quantity = ressources.amount;
                foreach (var item in GameManager.Instance.inventoryManager.Items)
                {
                    // Debug.Log("Quantity of ressources needed: " +  quantity);

                    if (item != null)
                    {
                        // Debug.Log("looping in item " + item.CurrentItem.type + item.CurrentQuantity);
                        if (ressources.type == item.CurrentItem.type)
                        {
                            // Debug.Log("Item type is equal at ressources type : "+ressources.type);
                            if (item.CurrentQuantity >= quantity)
                            {
                                // Debug.Log("item.CurrentQuantity >= ressources.amount" + item.CurrentQuantity + " >= " + quantity);
                                // Debug.Log("Removing easy way");
                                //item.DecreaseQuantity(ressources.amount);
                                GameManager.Instance.inventoryManager.UseItem(ressources.type, quantity);
                                break;
                            }
                            else //inferieur
                            {
                                //Debug.Log("item.CurrentQuantity < ressources.amount" + item.CurrentQuantity + " < " + quantity);
                                if (quantity >= item.CurrentQuantity && item.CurrentQuantity > 0)
                                {
                                    // Debug.Log("Removing Hard way");
                                    quantity -= item.CurrentQuantity; //3
                                    GameManager.Instance.inventoryManager.UseItem(ressources.type, item.CurrentQuantity, false);
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Item is null");
                    }
                }
                // RemoveItemsFromInventory(ressources); 
            }
        }
    }
}
