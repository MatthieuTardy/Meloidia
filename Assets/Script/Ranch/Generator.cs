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
            int Rnumber = ranchManager.getCNNumberByType(crocNoteType);
            int delta = NumberOfCN - Rnumber;
            Mathf.Abs(delta);
            for (int i = 0; i < delta; i++) 
            {
                Instantiate(ressourcesPrefabs, spawnPoint);
            }
            NumberOfCN += Rnumber;

            NumberOfCNText.text = NumberOfCN.ToString();
        }
    }
    
}
