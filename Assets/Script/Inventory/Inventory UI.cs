using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject[] UISlots;
    [SerializeField] GameObject UIContaineur;
    [SerializeField] TextMeshProUGUI[] UINumber;
    private void Start()
    {
    }

    private void LateUpdate()
    {
        UpdateInventoryVisuals();
    }
    void UpdateInventoryVisuals()
    {
        if (GameManager.Instance.inventoryManager)
        {
            bool OneItem = false;
            for (int i = 0; i < UISlots.Length; i++)
            {
                if (GameManager.Instance.inventoryManager.Items[i] == null)
                {
                    UISlots[i].SetActive(false);
                }
                else
                {
                    OneItem = true;
                    UIContaineur.SetActive(true);
                    UISlots[i].SetActive(true);
                    UISlots[i].GetComponent<Image>().sprite = GameManager.Instance.inventoryManager.Items[i].CurrentItem.sprite;
                    UINumber[i].text = GameManager.Instance.inventoryManager.Items[i].CurrentQuantity.ToString();
                }
            }

            if (!OneItem)
            {
                UIContaineur.SetActive(false);
            }
        
        }
    }
}
