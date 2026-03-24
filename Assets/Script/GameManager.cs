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
        //legumeManagerList = new List<LegumeManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        buildManager = FindAnyObjectByType<BuildManager>();
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    public void AddCrocNote(LegumeManager legumeManager)
    {
        Debug.Log("CrocNoteAdded " + legumeManager.name);
        legumeManagerList.Add(legumeManager);
    }

    public void RemoveCrocNote(LegumeManager legumeManager)
    {
        Debug.Log("CrocNoteRemoved " + legumeManager.name);
        legumeManagerList.Remove(legumeManager);
    }

    public void Quitter()
    {
        Application.Quit();
    }
}
