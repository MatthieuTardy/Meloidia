using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCondition : Condition
{
    [SerializeField] RessourceAmount[] RessourcesNeed;



    [SerializeField] bool Useitems;

    bool haveItem()
    {
        bool HaveItem = false;

        int quantity = 0;
        if (GameManager.Instance.inventoryManager != null && GameManager.Instance.inventoryManager.Items.Count > 0)
        {
            foreach (var ressources in RessourcesNeed)
            {
                bool containsRessources = false;
                foreach (var item in GameManager.Instance.inventoryManager.Items)
                {
                    if (item != null)
                    {
                        if (ressources.type == item.CurrentItem.type)
                        {
                            containsRessources = true;
                            // Debug.Log("HaveItemInInventory");
                            if (item.CurrentQuantity >= ressources.amount)
                            {
                                // Debug.Log("HaveQuantityNeeded");
                                HaveItem  = true;
                                break;
                            }
                            else
                            {
                                quantity += item.CurrentQuantity;
                                if (quantity >= ressources.amount)
                                {
                                    HaveItem = true;
                                }
                                else
                                {
                                    HaveItem = false;
                                }
                            }
                        }
                    }
                }
                //Debug.Log("ressources : " + ressources.type + " is " + containsRessources);
                if (!containsRessources)
                {
                    return false;
                }
            }
        }
        // Debug.LogWarning("il a pas les items");
        return HaveItem;
    }

    public override bool CheckCondition()
    {
        if (haveItem())
        {
            ConditionMet = true;
            if (Useitems)
            {
                if (GameManager.Instance.inventoryManager != null && GameManager.Instance.inventoryManager.Items.Count > 0)
                {
                    //Debug.LogWarning("Pass in RemoveItemFromInventory");
                    foreach (var ressources in RessourcesNeed)
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
                                        GameManager.Instance.inventoryManager.UseItem(ressources.type, quantity);
                                        break;
                                    }
                                    else //inferieur
                                    {
                                        //Debug.Log("item.CurrentQuantity < ressources.amount" + item.CurrentQuantity + " < " + quantity);
                                        if (quantity >= item.CurrentQuantity && item.CurrentQuantity > 0)
                                        {
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
                    }
                }
            }
        }
        return ConditionMet;
    }
}

