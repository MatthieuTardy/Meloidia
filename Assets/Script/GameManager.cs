using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    public PlayerManager playerManager;
    public List<LegumeManager> legumeManager;
    public BuildManager buildManager;
    public InventoryManager inventoryManager;
    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        buildManager = FindAnyObjectByType<BuildManager>();
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    public void AddCrocNote()
    {

    }
}
