using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Generator : Interractable 
{
    [SerializeField] CrocNoteType crocNoteType;
    [SerializeField] RanchManager ranchManager;

    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject ressourcesPrefabs;
    [SerializeField] TextMeshProUGUI NumberOfCNText;

    int NumberOfCN = 0;
    public override void Interract()
    {
        SpawnRessources();
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

            NumberOfCNText.text = NumberOfCN.ToString();
        }
    }
    
}
