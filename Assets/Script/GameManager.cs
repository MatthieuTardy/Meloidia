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
    public List<LegumeManager> legumeManagerList;
    public BuildManager buildManager;
    public InventoryManager inventoryManager;
    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        legumeManagerList = new List<LegumeManager>();
        buildManager = FindAnyObjectByType<BuildManager>();
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    public void AddCrocNote(LegumeManager legumeManager)
    {
        legumeManagerList.Add(legumeManager);
    }

    public void RemoveCrocNote(LegumeManager legumeManager)
    {
        legumeManagerList.Remove(legumeManager);
    }

    public void Quitter()
    {
        Application.Quit();
    }
}
