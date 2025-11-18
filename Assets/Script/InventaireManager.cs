using System.Collections.Generic;

namespace Inventory
{
    public abstract class Item
    {

    }

    public class Legume : Item
    {

    }

    public class Tool : Item
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
    }

    public class Inventory
    {
        public IReadOnlyList<ItemSlot> Items => items.AsReadOnly();
        private List<ItemSlot> items;

        private int InventoryMaxSize;

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
                items.Add(new ItemSlot(newItem, 1));
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