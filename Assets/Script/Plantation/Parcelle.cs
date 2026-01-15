using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Parcelle : MonoBehaviour
{
    [Header("Plantation data")]
    public List<GameObject> currentCrocNote;
    [SerializeField] GameObject RessourcesObject;
    [SerializeField] Transform RessourcesSpawn;
    [SerializeField] float TimeForCreate;
    [SerializeField] int LevelOfVillage;

    Coroutine ressourceRoutine; 
    
    [Header("Village1")]
    [SerializeField] int MaxCrocNote1;
    [SerializeField] GameObject[] Houses1;

    [Header("Village2")]
    [SerializeField] GameObject[] Houses2;
    [SerializeField] int MaxCrocNote2;

    private void Start()
    {
        currentCrocNote = new List<GameObject>();
    }

    public void AddCrocNoteToParcelle(GameObject crocNote)
    {
        currentCrocNote.Add(crocNote);
        if(LevelOfVillage == 1)
        {
            Instantiate(Houses1[currentCrocNote.Count - 1], this.transform);
        }
        else if(LevelOfVillage == 2)
        {
            Instantiate(Houses2[currentCrocNote.Count - 1], this.transform);
        }
        ParcelleEvolution();
    }

    void ParcelleEvolution()
    {
        if(currentCrocNote.Count == MaxCrocNote1)
        {
            StartRessourceRoutine();
        }
    }

    public void SetVillageLevel(int newLevel)
    {
        LevelOfVillage = newLevel;
    }

    public void StartRessourceRoutine()
    {
        StartCoroutine(RessourceRoutine());
    }

    IEnumerator RessourceRoutine()
    {
        yield return new WaitForSeconds(TimeForCreate);
        Instantiate(RessourcesObject,RessourcesSpawn.position,RessourcesSpawn.rotation,RessourcesSpawn);
        StartRessourceRoutine();
    }
   

}