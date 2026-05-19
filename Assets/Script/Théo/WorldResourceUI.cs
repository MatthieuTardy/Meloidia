using UnityEngine;
using TMPro;

[System.Serializable]
public struct ResourceTracker
{
    public TypeOfRessources ressourceToTrack;
    public TMP_Text worldText;
}

public class WorldResourceUI : MonoBehaviour
{
    [SerializeField] private ResourceTracker[] resourcesToTrack;

    private void Update()
    {
        foreach (var tracker in resourcesToTrack)
        {
            int quantity = 0;

            if (GameManager.Instance != null && GameManager.Instance.inventoryManager != null && GameManager.Instance.inventoryManager.Items.Count > 0)
            {
                foreach (var item in GameManager.Instance.inventoryManager.Items)
                {
                    if (item != null)
                    {
                        if (tracker.ressourceToTrack == item.CurrentItem.type)
                        {
                            quantity += item.CurrentQuantity;
                        }
                    }
                }
            }

            if (tracker.worldText != null)
            {
                tracker.worldText.text = quantity.ToString();
            }
        }
    }
}