using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Seed", menuName = "Custom/Seed Data")]
public class Seed : Vegetable_Scriptable 
{

    [Header("Seed Stats")]

    [SerializeField] private int growthTimeTotal;

    [SerializeField] private int happyness;
    [SerializeField] private int happynessNeed;

    public int GrowthTimeTotal => growthTimeTotal;
    public int Happyness => happyness;
    public int HappynessNeed => happynessNeed;


    [Header("Growth Stages")]

    [SerializeField] private int numberOfStages = 3;

    public Vegetable VegetableProductData;


    [Tooltip("Les assets visuels (Sprites, Prefabs, Tile) pour chaque stade. La taille doit être égale à numberOfStages.")]
    public GameObject[] stageVisuals;

    [HideInInspector] 
    public int ActualGrowthTime = 0;

    [HideInInspector] 
    public int CurrentStage = 0;


    public int CalculateCurrentStage(int actualGrowthTime)
    {
        if (growthTimeTotal <= 0 || numberOfStages <= 1) return 0;
        float timePerTransition = (float)growthTimeTotal / (numberOfStages - 1);
        int stage = Mathf.FloorToInt(actualGrowthTime / timePerTransition);

        return Mathf.Min(stage, numberOfStages - 1);
    }
}