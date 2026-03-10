using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    public IReadOnlyList<ItemSlot> Items => items.AsReadOnly();
    [SerializeField]private List<ItemSlot> items;
    public int InventorySize = 5;
    public void Start()
    {
        dictionaryOfItem = new Dictionary<Sprite, GameObject>();
        InitDictionnary();
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
    ///             si item == stack max || si inventaire rempli on destroy pas et on ajoute pas
    /// </summary>


    //si il existe pas, on check si y'a de la place
    // si il y a de la place -> on l'ajouter
    #region bool manager

    public (bool,int) CanStackItem(Item newItem)
    {
        for(int i=0;i<items.Count;i++)
        {
            if (items[i] != null) // si un item existe
            {
                if (items[i].CurrentItem.type == newItem.type) // si item deja dans l'inventaire
                {
                    if (items[i].CurrentQuantity < newItem.MaxStack) //si on a pas un stack
                    {
                        return (true, i);
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
    
    #endregion
    public bool TryToPickUp(Item newItem)
    {
        bool succesToPickUp = false;
        int amount = newItem.amount;
        //Debug.Log("try to pick up in inventory");

        (bool canStack, int index) = CanStackItem(newItem);
        //check si il existe une occurance de l'item && si on peut l'add
        if (canStack)   
        {
            //Debug.Log("Can Stack");
            // si on peut l'ajouter on l'ajoute
            AddItemToExistingSlot(newItem,amount,index);
            succesToPickUp = true;
        }
        else
        {
            //Debug.Log("CANNOT Stack");
            if (HaveSlotAvailable())
            {
                //Debug.Log("HaveSlotAvailable");
                AddItemToNewSlot(newItem,amount);
                succesToPickUp = true;
            }
        }
        if (succesToPickUp) { newItem.OnPickUp(); }
        return succesToPickUp;
    }
    void BrowseInventory()
    {
        if (Input.GetButtonDown("Scroll"))
        {
            int Scroll = (int)Input.GetAxisRaw("Scroll");
            if (Scroll != 0)
            {
                Debug.ClearDeveloperConsole();
                Debug.Log(" Scroll = " + Scroll + "\n \n \n");
                List<ItemSlot> NewItems = new List<ItemSlot>();
                for (int i = 0; i < InventorySize; i++)
                {
                    NewItems.Add(null);
                }
                for (int i = 0; i < items.Count; i++)
                {
                    if(items[i] != null)
                    {
                        Debug.Log("Item " + i + " = " +items[i].CurrentItem.type);
                    }
                    else
                    {
                        Debug.Log("Item " + i + " = none");
                    }

                    int nextIndex = i + Scroll;
                    if (nextIndex < 0)
                    {
                        nextIndex = InventorySize - 1;
                    }
                    else if (nextIndex >= InventorySize)
                    {
                        nextIndex = 0;
                    }
                    Debug.Log("next index = " + nextIndex);
                    if(items[nextIndex] != null)
                    {
                        NewItems[i] = items[nextIndex];
                    }
                    if(NewItems[i] != null)
                    {
                        Debug.Log("NewItem after change " + i + " = " + NewItems[i].CurrentItem.type);
                    }
                    else
                    {
                        Debug.Log("NewItem after change " + i + " = none");
                    }
                        


                    
                }
                for(int i = 0;i < NewItems.Count; i++)
                {
                    items[0] = NewItems[0];
                }
            }/*
            if (items[0] != null)
            {
                Debug.Log("Item " + 0 + " = " + items[0].CurrentItem.type);
            }
            else
            {
                Debug.Log("Item " + 0 + " = none");
            }*/
            //Debug.Log("Item 0 " + items[0].CurrentItem.type);
        }
    }

    #region adding item


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

    #endregion
    #region removing item

    public void UseItem(TypeOfRessources type, int amount, bool Merge = true)
    {
        /*
        foreach (ItemSlot itemSlot in items)
        {
            if (itemSlot.Item.Equals(item.Item))
            {
                item.DecreaseQuantity(1);
                // Gerer cas quantité = 0
            }
        }
        */

        for (int i = 0; i < items.Count; i++) 
        {
            if (items[i] != null)
            {
                if (items[i].CurrentQuantity >= amount)
                {

                    if (items[i].CurrentItem.type == type)
                    {
                        items[i].DecreaseQuantity(amount);
                        break;
                    }
                }
            }
        }
        if (Merge)
        {
            MergeInventory();
        }
    }

    private void Update()
    {
        BrowseInventory();
    }

    void DeleteItemIfZeroQuantity()
    {
        for (int i = 0; i < items.Count; i++) 
        {
            if (items[i] != null && items[i].CurrentQuantity <= 0)
            {
               // Debug.Log("Item " + i + " est devenu vide");
                items[i] = null;
            }
        }
    }
    public void MergeInventory()
    {

        // parcours l'inventaire
        DeleteItemIfZeroQuantity();
        ItemSlot slot = null;
        foreach (var item in items)
        {
            if(item != null)
            {
                // si l'item est pas au stack max
                if(item.CurrentQuantity < item.CurrentItem.MaxStack && item.CurrentQuantity > 0)
                {
                    slot = item;
                    foreach(var otheritem in items)
                    {
                        if (otheritem != null && otheritem != slot) 
                        {
                            // si il y a un autre item du meme type
                            if(otheritem.CurrentItem.type == slot.CurrentItem.type)
                            {
                                // si cet item est pas au max stack
                                if(item.CurrentQuantity < item.CurrentItem.MaxStack && item.CurrentQuantity > 0)
                                {
                                    // si le total des deux est <= au stack max, on les ajoutes
                                    if(otheritem.CurrentQuantity + item.CurrentQuantity <= item.CurrentItem.MaxStack)
                                    {
                                        item.IncreaseQuantity(otheritem.CurrentQuantity);
                                        otheritem.DecreaseQuantity(otheritem.CurrentQuantity);
                                        // on remplace un stack par null
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        DeleteItemIfZeroQuantity();

    }

    [SerializeField] CustomDictionnary[] DictionaryOfItem;
    Dictionary<Sprite,GameObject> dictionaryOfItem;

    public void DropItem(int index)
    {
        Debug.Log("Item = " + Items[index]);
        if(Items[index] != null)
        {
            GameObject obj = dictionaryOfItem.GetValueOrDefault(Items[index].CurrentItem.sprite);
            Vector3 pos = GameManager.Instance.playerManager.transform.position + Vector3.forward;
            Instantiate(obj,pos,Quaternion.identity);
            UseItem(Items[index].CurrentItem.type, 1);
        }
    }
    #endregion

    void InitDictionnary()
    {
        foreach(var value in DictionaryOfItem)
        {
            dictionaryOfItem.Add(value.key, value.value);
        }
    }

    public int GetRandomValableIndex()
    {
        int ran = UnityEngine.Random.Range(0, Items.Count);
        
        for(int i = ran; i >= 0; i--)
        {
            if(items[i] != null)
            {
                return i;
            }

        }
        return -1;

        
        
    }
}


[Serializable]
class CustomDictionnary
{
    [SerializeField] public Sprite key;
    public Sprite Key => key;
    [SerializeField] public GameObject value;
    public GameObject Value => value;
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
        //Debug.Log("On pickup " + this.gameObject.name);
        if(this != null)
        {
            Destroy(this.gameObject);
        }
    }
}

public enum TypeOfRessources
{
    Copeaux = 1,
    ressourceB = 2,
    ressourceC = 3,
    ressourceD = 4,
    ressourceE = 5,
    Graine_Carotte = 6,
    seedB = 7,
    seedC = 8,
    seedD = 9,
    seedE = 10,
    Cabais,
}

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
        GameManager.Instance.inventoryManager.MergeInventory();
    }
}
#endregion

