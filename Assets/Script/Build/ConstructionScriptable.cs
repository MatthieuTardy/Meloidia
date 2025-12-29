using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ConstructionData", menuName = "ScriptableObjects/Construction", order = 1)]
public class ConstructionScriptable : ScriptableObject
{
    [SerializeField] GameObject meshPrefab;
    [SerializeField] RessourceAmount[] ressourcesNeeded;



    public GameObject MeshPrefab => meshPrefab;
    public RessourceAmount[] RessourceNeeded => ressourcesNeeded;
}

[System.Serializable]
public class RessourceAmount
{
    public TypeOfRessources type;
    public int amount;
}



