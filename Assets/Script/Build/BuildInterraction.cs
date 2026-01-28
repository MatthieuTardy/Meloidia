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
    
    // --- REFERENCE AU PLAYER ---
    private PlayerController playerController;

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
        // On cherche le PlayerController dans la scène
        playerController = FindObjectOfType<PlayerController>();
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
            construct.GetComponentInChildren<Collider>().isTrigger = true;
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
        Debug.Log("Try to build");
        Debug.Log("Can build = " + CanBuild());
        Debug.Log("Have Item = " + HaveItemInInventory());
        if (CanBuild() && HaveItemInInventory())
        {
            {
                Build(constructChosen);
                Debug.Log("Builded");
                RemoveItemFromInventory();
                
                // --- DECLENCHEMENT ANIMATION ---
                if (playerController != null)
                {
                    playerController.TriggerBuildAnimation();
                }
            }
        }
    }
    
    bool CanBuild()
    {
        if (construct != null)
        {
            Vector3 halfExtents = construct.transform.localScale * 0.5f;

            Collider[] hits = Physics.OverlapBox(
                construct.transform.position,
                halfExtents,
                construct.transform.rotation
            );
            Transform parentToIgnore = transform.parent;
            foreach (Collider hit in hits)
            {
                // Ignore si le collider appartient au construct ou  ses enfants
                if (hit.transform.IsChildOf(construct.transform) || hit.transform.IsChildOf(parentToIgnore))
                    continue;

                // Sinon, c'est bloqu
                Debug.Log(hit.gameObject.name);
                return false;
            }

            return true;
        }
        else
        {
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

        //Toutes les ressources sont prsentes en quantit suffisante
        return true;
        */
        bool HaveItem = false;

        int quantity = 0;
        if (GameManager.Instance.inventoryManager != null && GameManager.Instance.inventoryManager.Items.Count > 0 && constructChosen != null)
        {
            // Debug.Log("pass the if");
            foreach (var ressources in constructChosen.RessourceNeeded)
            {
                foreach (var item in GameManager.Instance.inventoryManager.Items)
                {
                    if (item != null)
                    {
                        if (ressources.type == item.CurrentItem.type)
                        {
                            // Debug.Log("HaveItemInInventory");
                            if (item.CurrentQuantity >= ressources.amount)
                            {
                                // Debug.Log("HaveQuantityNeeded");
                                return true;
                            }
                            else
                            {
                                quantity += item.CurrentQuantity;
                                if (quantity >= ressources.amount)
                                {
                                    HaveItem = true;
                                }
                                else
                                {
                                    HaveItem = false;
                                }
                            }
                        }
                    }
                }
            }
        }
        // Debug.LogWarning("il a pas les items");
        return HaveItem;
    }

    void RemoveItemFromInventory() // search on one slot
    {
        if (GameManager.Instance.inventoryManager != null && GameManager.Instance.inventoryManager.Items.Count > 0 && constructChosen != null)
        {
           // Debug.LogWarning("Pass in RemoveItemFromInventory");
            foreach (var ressources in constructChosen.RessourceNeeded)
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
                                GameManager.Instance.inventoryManager.UseItem(ressources.type,quantity);
                                break;
                            }
                            else //inferieur
                            {
                                //Debug.Log("item.CurrentQuantity < ressources.amount" + item.CurrentQuantity + " < " + quantity);
                                if (quantity >= item.CurrentQuantity && item.CurrentQuantity > 0)
                                {
                                   // Debug.Log("Removing Hard way");
                                    quantity -= item.CurrentQuantity; //3
                                    GameManager.Instance.inventoryManager.UseItem(ressources.type,item.CurrentQuantity,false);
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
    
    void Build(ConstructionScriptable Build)
    {
        Vector3 buildPos = new Vector3(construct.transform.position.x, floorPosition, construct.transform.position.z);
        Instantiate(Build.MeshPrefab, buildPos,construct.transform.rotation);
        GameManager.Instance.buildManager.EndBuild();
    }

    void ChangeMat(bool good)
    {
        if (construct != null)
        {
            //Debug.Log(good);
            if (good)
            {
                construct.GetComponentInChildren<MeshRenderer>().material = goodMat;
            }
            else
            {
                construct.GetComponentInChildren<MeshRenderer>().material = wrongMat;
            }
        }
    }
}