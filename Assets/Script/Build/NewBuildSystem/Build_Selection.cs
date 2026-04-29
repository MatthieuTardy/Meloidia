using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    int SelectedIndex = -1;
    public void SelectIndex(int index)
    {
        SelectedIndex = index;
        ChangeText(index);
    }

    [SerializeField] TextMeshProUGUI NameBuild, Ressouces1, Ressources2, Ressouces3, Ressources4, Quantity1, Quantity2, Quantity3, Quantity4;
    public void ChangeText(int index)
    {
        NameBuild.text = construction[index].MeshPrefab.name;


        if (construction[index].RessourceNeeded.Length > 0)
        {
            if (construction[index].RessourceNeeded[0] != null)
            {
                Ressouces1.text = construction[index].RessourceNeeded[0].type.ToString();
                int quantityNeed = construction[index].RessourceNeeded[0].amount;
                Debug.Log("item 1");
                int quantityGet = GameManager.Instance.inventoryManager.GetQuantity(construction[index].RessourceNeeded[0].type);
                Quantity1.text = quantityNeed.ToString() + "/" + quantityGet.ToString();

            }
        }
        else
        {
            Ressouces1.text = "";
            Quantity1.text = "";
        }

        if (construction[index].RessourceNeeded.Length > 1)
        {

            if (construction[index].RessourceNeeded[1] != null)
            {
                Ressources2.text = construction[index].RessourceNeeded[1].type.ToString();
                int quantityNeed = construction[index].RessourceNeeded[1].amount;
                Debug.Log("item 2");
                int quantityGet = GameManager.Instance.inventoryManager.GetQuantity(construction[index].RessourceNeeded[1].type);
                Quantity2.text = quantityNeed.ToString() + "/" + quantityGet.ToString();
            }
        }
        else
        {
            Ressources2.text = "";
            Quantity2.text = "";
        }

        if (construction[index].RessourceNeeded.Length > 2)
        {

            if (construction[index].RessourceNeeded[2] != null)
            {
                Ressouces3.text = construction[index].RessourceNeeded[2].type.ToString();
                int quantityNeed = construction[index].RessourceNeeded[2].amount;
                int quantityGet = GameManager.Instance.inventoryManager.GetQuantity(construction[index].RessourceNeeded[2].type);
                Quantity3.text = quantityNeed.ToString() + "/" + quantityGet.ToString();
            }
           

        }
        else
        {
            Ressouces3.text = "";
            Quantity3.text = "";
        }

        if (construction[index].RessourceNeeded.Length > 3)
        {
            if (construction[index].RessourceNeeded[3] != null)
            {
                Ressources4.text = construction[index].RessourceNeeded[3].type.ToString();
                int quantityNeed = construction[index].RessourceNeeded[3].amount;
                int quantityGet = GameManager.Instance.inventoryManager.GetQuantity(construction[index].RessourceNeeded[3].type);
                Quantity4.text = quantityNeed.ToString() + "/" + quantityGet.ToString();
            }
        }
        else
        {
            Ressources4.text = "";
            Quantity4.text = "";
        }


    }
    public void ConstructChoosen()
    {
        Debug.Log("index : " + SelectedIndex);

        if (construction[SelectedIndex] != null)
        {
            Debug.Log("aaaaa");
            Debug.Log(BuildAlreadySelected + " = false");
            Debug.Log(HaveItemInInventory(construction[SelectedIndex]) + " = false");
            if (!BuildAlreadySelected && HaveItemInInventory(construction[SelectedIndex]))
            {
            Debug.Log("bbbbbb");
                clone = Instantiate(construction[SelectedIndex].MeshPrefab, BuildSpawn.position, BuildSpawn.rotation, BuildSpawn);
                BuildAlreadySelected = true;
                selectedConstruction = construction[SelectedIndex];
                RemoveItemFromInventory(construction[SelectedIndex]);
            }
        }
        else
        {
            Debug.Log("cccc");
            if (SelectedIndex == 4)
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
                Instantiate(item.RessourcePrefab, item.RessourcePrefab.transform.position, item.RessourcePrefab.transform.rotation);
            }
        }
    }

    bool HaveItemInInventory(ConstructionScriptable construction)
    {
        bool HaveItem = false;

        int quantity = 0;
        if (GameManager.Instance.inventoryManager != null && GameManager.Instance.inventoryManager.Items.Count > 0 && construction != null)
        {
             Debug.Log("pass the if");
            foreach (var ressources in construction.RessourceNeeded)
            {
                Debug.Log("1");
                foreach (var item in GameManager.Instance.inventoryManager.Items)
                {
                Debug.Log("2");
                    if (item != null)
                    {
                Debug.Log("3");
                        if (ressources.type == item.CurrentItem.type)
                        {
                Debug.Log("4");
                             Debug.Log("HaveItemInInventory");
                            if (item.CurrentQuantity >= ressources.amount)
                            {
                                Debug.Log("HaveQuantityNeeded");
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
        Debug.LogWarning("il a pas les items");
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
