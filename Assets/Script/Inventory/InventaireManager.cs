using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public IReadOnlyList<ItemSlot> Items => items.AsReadOnly();
    private List<ItemSlot> items;

    private int InventoryMaxSize = 4;


    /// <summary>
    /// a faire:    destroy la ressources au pickup
    ///             ajouter l'item dans la liste (voir en debug)
    ///             gerer le count
    ///             afficher dans l'ui
    ///             afficher le count
    ///             si item == stack max || si inventaire rempli on destroy pas et on ajoute pas
    ///             
    ///         
    /// </summary>



    public bool IsThereSpaceInInventory(Item newItem)
    {
        bool foundExistingItem = false;
        foreach (ItemSlot itemSlot in items)
        {
            if (itemSlot.Item.Equals(newItem))
            {
                foundExistingItem = true;
                break;
            }
        }

        return foundExistingItem || items.Count < InventoryMaxSize;
    }

    public void Update()
    {
        /*
        if (Input.GetButtonDown("Fire2"))
        {
            if (Items[0] != null)
            {
                Debug.Log("Objet 1 est " + Items[0]);
            }
            if (Items[1] != null)
            {
                Debug.Log("Objet 2 est " + Items[1]);
            }
            if (Items[2] != null)
            {
                Debug.Log("Objet 3 est " + Items[2]);
            }
            if (Items[3] != null)
            {
                Debug.Log("Objet 4 est " + Items[3]);
            }
        }
        */
    }
    public void Start()
    {
        items = new List<ItemSlot>();
        Debug.Log(Items);

    }
    public void AddItem(Item newItem)
    {
        Debug.Log("new item" + newItem);
        bool foundExistingItem = false;
        foreach (ItemSlot itemSlot in items)
        {
            if (itemSlot.Item.Equals(newItem))
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
    public Item Item { get; private set; }
    public int Quantity { get; private set; }

    public ItemSlot(Item item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0) return;
        Quantity += amount;
    }


    public void DecreaseQuantity(int amount)
    {
        if (amount <= 0) return;


        Quantity -= amount;
        if (Quantity < 0)
        {
            Quantity = 0;
        }
    }
}

