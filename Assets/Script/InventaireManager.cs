using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;

//using System.Diagnostics;
using UnityEngine;

namespace Inventory
{
    public abstract class Item
    {
        
    }
    public class Legume : Item
    {
        
    }

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

    public class Inventory
    {
        public IReadOnlyList<ItemSlot> Items => items.AsReadOnly();
        private List<ItemSlot> items;

        private int InventoryMaxSize = 4;

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

        public void start()
        {
            AddItem(new Legume());
            Debug.Log(Items);
        }
        public void AddItem(Item newItem)
        {
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
    }
}