using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections; // Ajout pour IEnumerator

public class InventoryManager : MonoBehaviour
{
    public IReadOnlyList<ItemSlot> Items => items.AsReadOnly();
    [SerializeField] private List<ItemSlot> items;
    public int InventorySize = 7;

    // --- Ajout pour référence automatique à l'Animator du joueur ---
    private Animator playerAnimator; 
    private PlayerController playerController;

    public void Start()
    {
        dictionaryOfItem = new Dictionary<Sprite, GameObject>();
        InitDictionnary();
        InitInventory();

        // --- Recherche automatique du PlayerController et de l'Animator ---
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerAnimator = playerController.animator;
        }
    }

    public void InitInventory()
    {
        items = new List<ItemSlot>();
        for (int i = 0; i < InventorySize; i++)
        {
            items.Add(null);
        }
    }

    #region bool manager

    public (bool, int) CanStackItem(Item newItem)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
            {
                if (items[i].CurrentItem.type == newItem.type)
                {
                    if (items[i].CurrentQuantity < newItem.MaxStack)
                    {
                        return (true, i);
                    }
                }
            }
        }
        return (false, -1);
    }

    public bool HaveSlotAvailable()
    {
        foreach (ItemSlot slot in items)
        {
            if (slot == null)
            {
                return true;
            }
        }
        return false;
    }

    public int GetQuantity(TypeOfRessources type)
    {
        foreach (ItemSlot item in items)
        {
            if (item != null)
            {
                if (item.CurrentQuantity > 0)
                {
                    if (item.CurrentItem.type == type)
                    {
                        return item.CurrentQuantity;
                    }
                }
            }
        }

        return 0;
    }

    #endregion

    public bool TryToPickUp(Item newItem)
    {
        if (newItem == null) return false;

        bool succesToPickUp = false;
        int MaxAmount = newItem.amount;

        for (int i = 1; i <= MaxAmount; i++)
        {
            newItem.amount = 1;
            (bool canStack, int index) = CanStackItem(newItem);

            if (canStack)
            {
                AddItemToExistingSlot(newItem, 1, index);
                succesToPickUp = true;
            }
            else
            {
                if (HaveSlotAvailable())
                {
                    AddItemToNewSlot(newItem, 1);
                    succesToPickUp = true;
                }
            }
        }

        if (succesToPickUp)
        {
            newItem.OnPickUp();

            // --- Active l'anim "interact" puis la désactive (petit délai)
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("interact", true);
                StartCoroutine(ResetInteractBool());
            }
        }

        return succesToPickUp;
    }

    // Ajout : Coroutine pour remettre interact à false après un petit délai
    private IEnumerator ResetInteractBool()
    {
        yield return new WaitForSeconds(0.2f);
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("interact", false);
        }
    }

    private void Update()
    {
    }

    #region adding item

    public void AddItemToExistingSlot(Item newItem, int amount, int index)
    {
        items[index].IncreaseQuantity(amount);
    }

    public void AddItemToNewSlot(Item newItem, int amount)
    {
        int index = -1;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                index = i;
                break;
            }
        }

        items[index] = new ItemSlot(newItem, amount);
    }

    #endregion

    #region removing item

    public void UseItem(TypeOfRessources type, int amount, bool Merge = true)
    {
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

    void DeleteItemIfZeroQuantity()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].CurrentQuantity <= 0)
            {
                items[i] = null;
            }
        }
    }

    public void MergeInventory()
    {
        DeleteItemIfZeroQuantity();
        ItemSlot slot = null;
        foreach (var item in items)
        {
            if (item != null)
            {
                if (item.CurrentQuantity < item.CurrentItem.MaxStack && item.CurrentQuantity > 0)
                {
                    slot = item;
                    foreach (var otheritem in items)
                    {
                        if (otheritem != null && otheritem != slot)
                        {
                            if (otheritem.CurrentItem.type == slot.CurrentItem.type)
                            {
                                if (item.CurrentQuantity < item.CurrentItem.MaxStack && item.CurrentQuantity > 0)
                                {
                                    if (otheritem.CurrentQuantity + item.CurrentQuantity <= item.CurrentItem.MaxStack)
                                    {
                                        item.IncreaseQuantity(otheritem.CurrentQuantity);
                                        otheritem.DecreaseQuantity(otheritem.CurrentQuantity);
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
    Dictionary<Sprite, GameObject> dictionaryOfItem;

    public void DropItem(int index)
    {
        if (Items[index] != null)
        {
            GameObject obj = dictionaryOfItem.GetValueOrDefault(Items[index].CurrentItem.sprite);
            Items[index].CurrentItem.amount = 1;
            Vector3 pos = GameManager.Instance.playerManager.transform.position + (Vector3.forward * 2);
            Instantiate(obj, pos, Quaternion.identity);
            UseItem(Items[index].CurrentItem.type, 1);
        }
    }
    #endregion

    void InitDictionnary()
    {
        foreach (var value in DictionaryOfItem)
        {
            dictionaryOfItem.Add(value.key, value.value);
        }
    }

    public int GetRandomValableIndex()
    {
        int ran = UnityEngine.Random.Range(0, Items.Count);

        for (int i = ran; i >= 0; i--)
        {
            if (items[i] != null)
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
        if (this != null)
        {
            Destroy(this.gameObject);
        }
    }
}

public enum TypeOfRessources
{
    RessourceCarottes = 1,
    RessourceNavet = 2,
    RessourcePoivron = 3,
    ressourceChou = 4,
    ressourceBrocoli = 5,
    Graine_Carotte = 6,
    Graine_Navet = 7,
    Graine_Poivron = 8,
    Graine_Chou = 9,
    Graine_Brocoli = 10,
    Cabais = 11,
    Crystal = 12,
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