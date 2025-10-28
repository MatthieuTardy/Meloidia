using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Seed", menuName = "Custom/Seed Data")]
public class Seed : Vegetable_Scriptable 
{

    [Header("Seed Stats")]
    [Tooltip("Temps total (en ticks ou unités de jeu) nécessaire pour la croissance.")]
    [SerializeField] private int growthTimeTotal;
    [Tooltip("Influence sur le bonheur des plantes adjacentes (peut-être pas utilisé ici).")]
    [SerializeField] private int happynessInfluence;

    public int GrowthTimeTotal => growthTimeTotal;
    public int HappynessInfluence => happynessInfluence;

  
    [Header("Growth Stages")]
    [Tooltip("Nombre total de stades de croissance avant la maturité (inclut la graine et le légume final).")]
    [SerializeField] private int numberOfStages = 3;
    [Tooltip("Le ScriptableObject du légume final après la croissance.")]
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