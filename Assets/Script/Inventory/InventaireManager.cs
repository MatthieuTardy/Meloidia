using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    public IReadOnlyList<ItemSlot> Items => items.AsReadOnly();
    [SerializeField]private List<ItemSlot> items;
    public int InventorySize = 4;
    public void Awake()
    {
        InitInventory();
    }
    public void InitInventory()
    {
        items = new List<ItemSlot>();
        for (int i = 0; i < InventorySize; i++)
        {
            items.Add(null);
        }
    }
    /// <summary>
    /// a faire:    
    ///             si stack == max et place dispo on ajoute sur l'autre slot si possible, sinon on fait
    ///             si item == stack max || si inventaire rempli on destroy pas et on ajoute pas
    /// </summary>


    //si il existe pas, on check si y'a de la place
    // si il y a de la place -> on l'ajouter
    #region bool manager

    public (bool,int) CanStackItem(Item newItem)
    {
        //Debug.Log(items.Count);
        for(int i=0;i<items.Count;i++)
        {
            Debug.Log("trying to stack on slot " + i);
            if (items[i] != null) // si un item existe
            {
                Debug.Log("item "+ i +" is not null");
                if (items[i].CurrentItem.type == newItem.type) // si item deja dans l'inventaire
                {
                    Debug.Log("item " + i + "is same type as " + newItem + " : " + newItem.type);
                    if (items[i].CurrentQuantity < newItem.MaxStack) //si on a pas un stack
                    {
                        Debug.Log("item " + i + " is NOT at maxStack");
                        return (true,i);
                    }
                    else
                    {
                        Debug.Log("item " + i + " is at maxStack");
                        //return (false,-1);
                    }
                }
            }
        }
        return (false,-1);
    }
     
    public bool HaveSlotAvailable()
    {
        //Debug.Log(items.Count);
        foreach (ItemSlot slot in items)
        {
            if (slot == null)
            {
                return true;
            }
        }
        return false;
    }
    
    public void TryToPickUp(Item newItem)
    {
        int amount = newItem.amount;
        Debug.Log("try to pick up in inventory");

        (bool canStack, int index) = CanStackItem(newItem);
        //check si il existe une occurance de l'item && si on peut l'add
        if (canStack)   
        {
            Debug.Log("Can Stack");
            // si on peut l'ajouter on l'ajoute
            AddItemToExistingSlot(newItem,amount,index);
            newItem.OnPickUp();
        }
        else
        {
            Debug.Log("CANNOT Stack");
            if (HaveSlotAvailable())
            {
                Debug.Log("HaveSlotAvailable");
                AddItemToNewSlot(newItem,amount);
                newItem.OnPickUp();
            }
        }
    }
    #endregion

    public void AddItemToExistingSlot(Item newItem,int amount,int index)
    {
        /*
        Debug.Log("new item" + newItem);
        bool foundExistingItem = false;
        foreach (ItemSlot itemSlot in items)
        {
            if (itemSlot.CurrentItem.Equals(newItem))
            {
                foundExistingItem = true;
                itemSlot.IncreaseQuantity(1);
                break;
            }
        }

        if (!foundExistingItem)
        {
            items.Add(new ItemSlot(newItem, 1));
            Debug.Log("Ajout de " + newItem);
        }
        */

        items[index].IncreaseQuantity(amount);
    }
    
    public void AddItemToNewSlot(Item newItem,int amount)
    {
        int index = -1;
        for(int i = 0; i < items.Count;i++) 
        {
            if(items[i] == null)
            {
                index = i;
                break;
            }
        } // get the available slot;

        items[index] = new ItemSlot(newItem, amount);

    }

    /*
    public void UseItem(ItemSlot item)
    {
        foreach (ItemSlot itemSlot in items)
        {
            if (itemSlot.Item.Equals(item.Item))
            {
                item.DecreaseQuantity(1);
                // Gerer cas quantité = 0
            }
        }
    }
    */
}



#region Item Management
public class Item : MonoBehaviour
{
    public TypeOfRessources type;
    public int MaxStack;
    public int amount = 1;
    public Sprite sprite;
    public void OnPickUp()
    {
        Debug.Log("On pickup " + this.gameObject.name);
        Destroy(this.gameObject);
    }
}
public enum TypeOfRessources
{
    ressourceA = 1,
    ressourceB = 2,
    ressourceC = 3,
    ressourceD = 4,
    ressourceE = 5,
}
#endregion



public class ItemSlot
{
    public Item CurrentItem { get; private set; }
    public int CurrentQuantity { get; private set; }

    public ItemSlot(Item item, int quantity)
    {
        CurrentItem = item;
        CurrentQuantity = quantity;
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0) return;
        CurrentQuantity += amount;
    }


    public void DecreaseQuantity(int amount)
    {
        if (amount <= 0) return;


        CurrentQuantity -= amount;
        if (CurrentQuantity < 0)
        {
            CurrentQuantity = 0;
        }
    }
}

