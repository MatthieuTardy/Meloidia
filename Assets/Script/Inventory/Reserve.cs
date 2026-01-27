
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Reserve : MonoBehaviour
{
    [Header("Debug")]
    public KeyCode ScrollPositive;
    public KeyCode ScrollNegative;
    public KeyCode Switch;
    public KeyCode Validation;
    
    [SerializeField] int Capacity;

    
    public ItemSlot[] ItemsStocked;

    int currentIndex;
    public bool? StockingInReserve;

    [Header("UI")]
    [SerializeField] GameObject reserve_Canva;
    [SerializeField] List<GameObject> ReserveSlot;
    [SerializeField] List<GameObject> PlayerSlot;
    [SerializeField] GameObject CursorSelection;
    private void Start()
    {
        ItemsStocked = new ItemSlot[Capacity];
    }

    private void Update()
    {
        InputManagement();
        UpdateUI();
    }
    // Pour les plot de réserve, on pourra interagir avec
    public void OpenReserve()
    {
        Debug.Log("Reserve is Open");
        StockingInReserve = true;
        SelectItem(0);
        ShowUI();
    }

    public void CloseReserve()
    {
        Debug.Log("Reserve is Close");
        StockingInReserve = null;
        HideUI();
    }
    
    #region Capacity Management

    public void AddItemInStock(ItemSlot item)
    {
        Debug.Log("item = " +item);
        if (SlotAvailable(item) > -1)
        {
            if (ItemsStocked[SlotAvailable(item)] == null)
            {
                item.CurrentItem.amount = 1;
                ItemsStocked[SlotAvailable(item)] = new ItemSlot(item.CurrentItem, 1);   
            }
            else if (ItemsStocked[SlotAvailable(item)].CurrentItem.type == item.CurrentItem.type)
            {
                ItemsStocked[SlotAvailable(item)].IncreaseQuantity(1);
            }
            GameManager.Instance.inventoryManager.Items[currentIndex].DecreaseQuantity(1);
       }
    }

    public Item GetItemInStock(int index) 
    {
        return ItemsStocked[index].CurrentItem;
    }

    int SlotAvailable(ItemSlot item) // return index du slot qui est Disponible
    {
        for (int i = 0; i < ItemsStocked.Length; i++)
        {
            if (ItemsStocked[i] == null)
            {
                return i; 
            }
            else if (ItemsStocked[i].CurrentItem.type == item.CurrentItem.type)
            {
                if (ItemsStocked[i].CurrentQuantity < ItemsStocked[i].CurrentItem.MaxStack)
                {
                    return i;
                }
            }
            
        }
        return -1;
    }
    #endregion

    #region Interraction


    void InputManagement()
    {
        if (StockingInReserve != null) 
        {
            /*
            if (Input.GetKeyDown(ScrollPositive))
            {
                SelectItem(1);
            }
            else if (Input.GetKeyDown(ScrollNegative))
            {
                SelectItem(-1);
            }
            else if (Input.GetKeyDown(Switch)) 
            {
                SwitchInventory();
            }
            else if(Input.GetKeyDown(Validation)) 
            {
                ValidateSelection();
            }
            */

            if(Input.GetButtonDown("Scroll"))
            {
                if(Input.GetAxisRaw("Scroll") > 0)
                {
                    SelectItem(1);
                }
                else if(Input.GetAxisRaw("Scroll") < 0)
                {
                    SelectItem(-1);
                }
            }

            if (Input.GetButtonDown("Switch"))
            {
                SwitchInventory();
            }
            else if (Input.GetButtonDown("Jump"))
            {
                ValidateSelection();
            }

            if (Input.GetButtonDown("Cancel"))
            {
                CloseReserve();
            }
        }
    }
    // on l’on pourra placer/retirer(touche pour switch entre stocker et prendre,
    void SelectItem(int Nextindex)
    {
        Debug.Log("current index before " + currentIndex);
        // quand le joueur choisi de stocker,
        currentIndex += Nextindex;
        // il scroll d’abord dans son inventaire pour choisir l’item qu’il souhaite stocker.
        if (StockingInReserve == false) 
        {
            //change l'index
            if(currentIndex < 0)
            {
                currentIndex = ItemsStocked.Length - 1;
            }
            else if(currentIndex >= ItemsStocked.Length)
            {
                currentIndex = 0;
            }
        } //parcourir l'inventaire de la reserve

        // Si il choisi de prendre, il scrollera dans la reserver
        else if(StockingInReserve == true) 
        {
            if (currentIndex < 0)
            {
                currentIndex = GameManager.Instance.inventoryManager.Items.Count - 1;
            }
            else if (currentIndex >= GameManager.Instance.inventoryManager.Items.Count)
            {
                currentIndex = 0;
            }
        } // parcourir l'inventaire du joueur

        Debug.Log("Current index after " +  currentIndex);
    }

    void SwitchInventory()
    {
        StockingInReserve = !StockingInReserve;
        SelectItem(0);
    }

    // et choisira l’item qu’il souhaite prendre dans son inventaire),
    void ValidateSelection()
    {
        if(StockingInReserve == true)
        {
            if (GameManager.Instance.inventoryManager.Items[currentIndex] != null)
            {
                Debug.Log("Validate transfert of :" + GameManager.Instance.inventoryManager.Items[currentIndex].CurrentItem);
                AddItemInStock(GameManager.Instance.inventoryManager.Items[currentIndex]);
            }
        }

        else if(StockingInReserve == false)
        {
            if(ItemsStocked[currentIndex] != null)
            {
                bool CanAdd = GameManager.Instance.inventoryManager.TryToPickUp(GetItemInStock(currentIndex));
                if (CanAdd) 
                {
                    ItemsStocked[currentIndex].DecreaseQuantity(1);
                }
            }
        }
    }
    #endregion

    #region UI Management
    // et un inventaire de réserve apparait
    void UpdateUI()
    {
        if (StockingInReserve != null)
        {
            UpdatePlayerInventory();
            UpdateReserveInventory();
            SetCursorToSelectedItem();
        }
    }

    void ShowUI()
    {
        reserve_Canva.SetActive(true);
        
    }

    void HideUI()
    {
        reserve_Canva.SetActive(false);
    }

    void DeleteItemIfZeroQuantity()
    {
        for (int i = 0; i < ItemsStocked.Length; i++)
        {
            if (ItemsStocked[i] != null && ItemsStocked[i].CurrentQuantity <= 0)
            {
                // Debug.Log("Item " + i + " est devenu vide");
                ItemsStocked[i] = null;
            }
        }
    }

    void UpdatePlayerInventory()
    {
        if (GameManager.Instance.inventoryManager)
        {
            for (int i = 0; i < PlayerSlot.Count; i++)
            {
                if (GameManager.Instance.inventoryManager.Items[i] == null)
                {
                    //PlayerSlot[i].SetActive(false);
                }
                else
                {
                    //PlayerSlot[i].SetActive(true);
                    PlayerSlot[i].GetComponent<Image>().sprite = GameManager.Instance.inventoryManager.Items[i].CurrentItem.sprite;
                    PlayerSlot[i].GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.inventoryManager.Items[i].CurrentQuantity.ToString();
                }
            }
        }
    }

    void UpdateReserveInventory()
    {
        DeleteItemIfZeroQuantity();
        for (int i = 0; i < ReserveSlot.Count; i++)
        {
            if (ItemsStocked[i] == null)
            {
               // ReserveSlot[i].SetActive(false);
            }
            else
            {
               // ReserveSlot[i].SetActive(true);
                ReserveSlot[i].GetComponent<Image>().sprite = ItemsStocked[i].CurrentItem.sprite;
                ReserveSlot[i].GetComponentInChildren<TextMeshProUGUI>().text = ItemsStocked[i].CurrentQuantity.ToString();
            }
        }
    }

    void SetCursorToSelectedItem()
    {
        if (StockingInReserve == true)
        {
            CursorSelection.transform.position = PlayerSlot[currentIndex].transform.position;

        }
        else if (StockingInReserve == false) 
        {
            CursorSelection.transform.position = ReserveSlot[currentIndex].transform.position;
        }
    }
    #endregion


    
}
