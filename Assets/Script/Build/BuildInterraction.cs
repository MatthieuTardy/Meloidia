
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInterraction : MonoBehaviour
{
    public bool PlayerIsBuildMode;
    bool previewInstantiate;
    bool isBuilding = false;

    [Header("Construction Setting")]
    [Range(1,10)][SerializeField] int Ratio;
    [SerializeField] float floorPosition;

    public ConstructionScriptable constructChosen;
    GameObject construct;
    [SerializeField] Transform constructionSpawn;
    Transform Spawn;

    [SerializeField] Material goodMat;
    [SerializeField] Material wrongMat;

    private void Update()
    {
        if (PlayerIsBuildMode)
        {
            InstantiatePreViewBuild();
            PrevisualizeConstruct();
            ChangeMat(CanBuild());
            if (Input.GetButtonDown("Fire1"))
            {
                    TryToBuild();
            }
        }
        else
        {
            if(construct != null)
            {
                Destroy(construct);
            }
        }
    }
    private void Start()
    {
        
    }
    #region Build Preview
    void PrevisualizeConstruct()
    {
        if (construct != null)
        {
            construct.transform.position = SetOnGrid(Ratio);
            construct.transform.rotation = Quaternion.identity;
        }

    }

    public void InstantiatePreViewBuild()
    {
        if (construct == null && PlayerIsBuildMode)
        {
            construct = Instantiate(constructChosen.MeshPrefab, constructionSpawn.position, Quaternion.identity, constructionSpawn);
            previewInstantiate = true;
        }
        else if(construct != null && !PlayerIsBuildMode)
        {
            previewInstantiate = false;
        }
        
    }


    Vector3Int SetOnGrid(float gridSize)
    {
        Vector3 pos = constructionSpawn.position;

        return new Vector3Int(
            Mathf.RoundToInt(pos.x / gridSize) * (int)gridSize,
            Mathf.RoundToInt(pos.y),
            Mathf.RoundToInt(pos.z / gridSize) * (int)gridSize
        );
    }
    #endregion

    void TryToBuild()
    {
        if (CanBuild() && HaveItemInInventory())
        {
            {
                Build();
                RemoveItemFromInventory();
            }
        }
    }
    bool CanBuild()
    {
        if (construct != null)
        {
            Vector3 halfExtents = (construct.transform.localScale * 0.5f);
            bool blocked = Physics.CheckBox(construct.transform.position, halfExtents, construct.transform.rotation);
            if (blocked)
            {
                return false;
            }
            return true;
        }
        else
        {
            //Debug.LogError("CONSTRUCT == Null");
            return false;
        }


    }

    bool HaveItemInInventory()
    {
        /*
        if (GameManager.Instance.inventoryManager == null)
            return false;

        var inventory = GameManager.Instance.inventoryManager.Items;

        // Pour chaque ressource requise par la construction
        foreach (var needed in constructChosen.RessourceNeeded)
        {
            int totalAmountInInventory = 0;

            // On cherche cette ressource dans l'inventaire
            foreach (var item in inventory)
            {
                if (item.CurrentItem.type == needed.type) 
                {
                    totalAmountInInventory += item.CurrentQuantity;
                }
            }

            //Pas assez de cette ressource
            if (totalAmountInInventory < needed.amount)
                return false;
        }

        //Toutes les ressources sont présentes en quantité suffisante
        return true;
        */

        int quantity = 0;
        if (GameManager.Instance.inventoryManager != null && GameManager.Instance.inventoryManager.Items.Count > 0 && constructChosen != null)
        {
           // Debug.Log("pass the if");
            foreach (var ressources in constructChosen.RessourceNeeded)
            {
                foreach (var item in GameManager.Instance.inventoryManager.Items)
                {
                    if (ressources.type == item.CurrentItem.type)
                    {
                       // Debug.Log("HaveItemInInventory");
                        if (item.CurrentQuantity >= ressources.amount)
                        {
                            //Debug.Log("HaveQuantityNeeded");
                            return true;
                        }
                        else
                        {
                            quantity += item.CurrentQuantity;
                            if(quantity >= ressources.amount)
                            {
                               return true;
                            }
                        }
                    }
                }
            }
        }
      //  Debug.LogWarning("il a pas les items");
        return false;
    }

    void RemoveItemFromInventory() // search on one slot
    {
        if (GameManager.Instance.inventoryManager != null && GameManager.Instance.inventoryManager.Items.Count > 0 && constructChosen != null)
        {
            foreach (var ressources in constructChosen.RessourceNeeded)
            {
                foreach (var item in GameManager.Instance.inventoryManager.Items)
                {
                    if (item != null)
                    {
                        if (ressources.type == item.CurrentItem.type)
                        {
                            if (item.CurrentQuantity >= ressources.amount)
                            {
                                //Debug.Log("Removing");
                                //item.DecreaseQuantity(ressources.amount);
                                GameManager.Instance.inventoryManager.UseItem(ressources.type,ressources.amount);
                                return;
                            }
                        }
                    }
                }
               // RemoveItemsFromInventory(ressources); 
            }
        }
    }
     /*
    void RemoveItemsFromInventory(RessourceAmount RA) // search on more slot
    {
        int quantity = RA.amount;
        if (GameManager.Instance.inventoryManager != null && GameManager.Instance.inventoryManager.Items.Count > 0 && constructChosen != null)
        {
            
            foreach (var ressources in constructChosen.RessourceNeeded)
            {
                foreach (var item in GameManager.Instance.inventoryManager.Items)
                {
                    if (ressources.type == item.CurrentItem.type)
                    {
                        if (item.CurrentQuantity >= ressources.amount)
                        {
                            item.DecreaseQuantity(ressources.amount);
                            return;
                        }
                    }
                }
            }

            foreach (var item in GameManager.Instance.inventoryManager.Items)
            {
                if (item.CurrentItem.type == RA.type)
                {
                    if (item.CurrentQuantity < quantity)
                    {
                        Debug.Log("Removing 2");
                        quantity -= item.CurrentQuantity;
                        item.DecreaseQuantity(item.CurrentQuantity);
                    }
                    if (quantity <= 0)
                    {
                        return;
                    }
                }
            }
        }
    }
    */
    void Build()
    {
        Vector3 buildPos = new Vector3(construct.transform.position.x, floorPosition, construct.transform.position.z);
        Instantiate(construct, buildPos,construct.transform.rotation);
        GameManager.Instance.buildManager.EndBuild();
    }

    void ChangeMat(bool good)
    {
        if (construct != null)
        {

            //Debug.Log(good);
            if (good)
            {
                construct.GetComponent<MeshRenderer>().material = goodMat;
            }
            else
            {
                construct.GetComponent<MeshRenderer>().material = wrongMat;
            }
        }
    }
}
