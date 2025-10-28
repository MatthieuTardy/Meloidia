using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantingManager : MonoBehaviour
{
    [Header("Références Scène")]
    public Tilemap farmTilemap; // La Tilemap du sol cultivable
    public GameObject cropPrefab; // Le Prefab contenant le script Crop.cs

    [Header("Données de Plantation")]
    [Tooltip("La graine (ScriptableObject) actuellement sélectionnée dans l'inventaire du joueur.")]
    public Seed selectedSeed;

    // Collection pour suivre l'occupation des tuiles (CellPosition -> Crop Component)
    private Dictionary<Vector3Int, Crop> plantedCrops = new Dictionary<Vector3Int, Crop>();

    private void Update()
    {
        // Utilisation de l'Input classique pour la simplicité (compatible 2022 LTS)
        if (Input.GetMouseButtonDown(0)) // Détecte le clic gauche
        {
            TryPlantingOrHarvesting();
        }
    }

    private void TryPlantingOrHarvesting()
    {
        // 1. Trouver la position du clic dans le monde
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 2. Convertir en position de cellule Tilemap
        Vector3Int cellPosition = farmTilemap.WorldToCell(mouseWorldPos);

        // 3. Vérifier si la cellule est plantable (doit contenir une tuile)
        if (farmTilemap.GetTile(cellPosition) == null)
        {
            Debug.Log("Ce n'est pas une zone de plantation.");
            return;
        }

        // 4. Vérifier si la cellule est occupée
        if (plantedCrops.ContainsKey(cellPosition))
        {
            // --- Clic sur une plante existante (Récolte/Interaction) ---
            plantedCrops[cellPosition].Harvest();
            // Important : Si la récolte détruit l'objet, il faut le retirer du dictionnaire :
            if (plantedCrops[cellPosition] == null)
            {
                plantedCrops.Remove(cellPosition);
            }
        }
        else
        {
            // --- Plantation ---
            if (selectedSeed == null)
            {
                Debug.LogWarning("Veuillez sélectionner une graine !");
                return;
            }

            // Instancier la plante au centre de la tuile
            Vector3 plantWorldPos = farmTilemap.GetCellCenterWorld(cellPosition);
            GameObject newCropObject = Instantiate(cropPrefab, plantWorldPos, Quaternion.identity);

            // Initialiser le composant Crop
            Crop cropComponent = newCropObject.GetComponent<Crop>();
            if (cropComponent != null)
            {
                cropComponent.Initialize(selectedSeed);
                // Ajouter à la liste des plantes actives
                plantedCrops.Add(cellPosition, cropComponent);
                Debug.Log($"Graine de {selectedSeed.name} plantée à {cellPosition}.");
            }
            else
            {
                Debug.LogError("Le Prefab de culture doit avoir un script Crop!");
                Destroy(newCropObject);
            }
        }
    }

    // --- Fonction à appeler régulièrement par un Timer Global ---
    public void GlobalGrowthTick(int timeIncrease = 1)
    {
        // Fait grandir toutes les plantes
        foreach (var crop in plantedCrops.Values)
        {
            crop.TickGrowth(timeIncrease);
        }
    }
}