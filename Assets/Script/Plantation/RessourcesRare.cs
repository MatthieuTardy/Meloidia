using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourcesRare : Interractable
{
    [InfoBox("SpawnDuration est le temps que pour 1 ressources")]
    [SerializeField] float SpawnDuration;
    [SerializeField] int MaxStack;
    public int currentStack;


    [SerializeField] TypeOfRessources typeOfRessources;
    [SerializeField] Sprite ItemSprite;

    [SerializeField] GameObject[] RessourcesObjects;

    private void StartSpawnRoutine()
    {
        if (currentStack < MaxStack)
        {
            StartCoroutine(Spawn());
        }
    }
    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(SpawnDuration);
        ActivateRessources(currentStack,true);
        currentStack++;
        StartSpawnRoutine();
    }

    public override void Interract()
    {
        if(currentStack > 0 && GameManager.Instance.inventoryManager.HaveSlotAvailable())
        { 
            StopAllCoroutines();
            for (int i = 1; i <= currentStack; i++)
            {
                bool success = GameManager.Instance.inventoryManager.TryToPickUp(newRessources());
                if (success)
                {
                    Debug.Log("success to pick " + newRessources().type  +" " +i) ;
                    currentStack--;
                    ActivateRessources(currentStack, false);
                }
            }
            StartSpawnRoutine();
        }
    }



    Ressources newRessources()
    {
        Ressources r = new Ressources();
        r.type = typeOfRessources;
        r.amount = 1;
        r.MaxStack = MaxStack;
        r.sprite = ItemSprite;
        return r;
    }
    void ActivateRessources(int index, bool actif)
    {
        Debug.Log(index);
        RessourcesObjects[index].SetActive(actif);
    }
}


