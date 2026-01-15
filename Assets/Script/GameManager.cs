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
    public LegumeManager legumeManager;
    public List<LegumeManager> legumeManagerlist;
    public BuildManager buildManager;
    public InventoryManager inventoryManager;
    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        legumeManager = FindObjectOfType<LegumeManager>();
        buildManager = FindAnyObjectByType<BuildManager>();
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    public void AddCrocNote()
    {

    }
}
