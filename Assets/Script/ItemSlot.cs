using System;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{

    public string ItemName { get; private set; } 
    public int Quantity { get; private set; } = 0;

    public bool IsEmpty => string.IsNullOrEmpty(ItemName);

    public ItemSlot()
    {
        ClearSlot();
    }

    /// <summary>
    /// Ajoute ou remplace un objet dans l'emplacement.
    /// </summary>
    public void SetItem(string name, int count)
    {
        this.ItemName = name;
        this.Quantity = count;
        // Déclenchez ici un événement pour mettre à jour l'icône dans l'UI
    }

    /// <summary>
    /// Retire tout le contenu de l'emplacement.
    /// </summary>
    public void ClearSlot()
    {
        this.ItemName = null;
        this.Quantity = 0;
    }
}