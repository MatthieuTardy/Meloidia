using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ConstructionData", menuName = "ScriptableObjects/Construction", order = 1)]
public class ConstructionScriptable : ScriptableObject
{
    [SerializeField] GameObject meshPrefab; 


    public GameObject MeshPrefab => meshPrefab;
}
