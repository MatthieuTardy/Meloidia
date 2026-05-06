using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Generator : MonoBehaviour 
{
    [SerializeField] CrocNoteType crocNoteType;
    [SerializeField] RanchManager ranchManager;

    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject ressourcesPrefabs;
    [SerializeField] TextMeshProUGUI NumberOfCNText;

    int NumberOfCN = 0;


    private void FixedUpdate()
    {
        ShowNumberInUI();
    }
    void ShowNumberInUI()
    {
        NumberOfCNText.text = ranchManager.getCNNumberByType(crocNoteType).ToString();
        if(NumberOfCN != ranchManager.getCNNumberByType(crocNoteType))
        {
            SpawnRessources();
        }
    }
    void SpawnRessources()
    {
        if(ranchManager.getCNNumberByType(crocNoteType) > 0)
        {
            Debug.Log("Number of CN = " + NumberOfCN);
            int Rnumber = ranchManager.getCNNumberByType(crocNoteType);
            Debug.Log("Rnumber = " + Rnumber);
            int delta = NumberOfCN - Rnumber; 
            delta = Mathf.Abs(delta);
            Debug.Log("delta = " + delta);
            for (int i = 0; i < delta; i++) 
            {
                Instantiate(ressourcesPrefabs, spawnPoint.position,spawnPoint.rotation);
            }
            NumberOfCN += delta;
            Debug.Log("Number of CN = " + NumberOfCN);

            
        }
    }
    
}
