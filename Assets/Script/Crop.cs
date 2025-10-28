using UnityEngine;

// Ce script doit être sur le Prefab instancié lors de la plantation
public class Crop : MonoBehaviour
{
    private Seed _seedData;
    private int _actualGrowthTime = 0;
    private int _currentStage = 0;

    [Header("Référence Visuelle")]
    [Tooltip("Le conteneur des visuels (enfant de ce GameObject) qui sera mis à jour.")]
    public Transform visualContainer; // Pour y instancier les modèles/sprites du stade

    private GameObject _currentVisualInstance;

    /// <summary>
    /// Initialise la plante avec les données de la graine.
    /// </summary>
    public void Initialize(Seed seed)
    {
        _seedData = seed;
        _actualGrowthTime = 0;
        _currentStage = 0;

        if (visualContainer == null)
        {
            Debug.LogError("Crop: visualContainer n'est pas assigné. Assurez-vous d'avoir un enfant pour les visuels.");
        }

        UpdateVisual();
    }

    /// <summary>
    /// Met à jour l'état de la plante (à appeler par un Game Manager à intervalle régulier).
    /// </summary>
    public void TickGrowth(int timeIncrease = 1)
    {
        if (_seedData == null || _actualGrowthTime >= _seedData.GrowthTimeTotal)
            return; // Déjà mûr ou non initialisé

        _actualGrowthTime += timeIncrease;

        int newStage = _seedData.CalculateCurrentStage(_actualGrowthTime);

        if (newStage != _currentStage)
        {
            _currentStage = newStage;
            UpdateVisual();
        }

        if (_actualGrowthTime >= _seedData.GrowthTimeTotal)
        {
            Debug.Log($"{_seedData.name} est prête à être récoltée !");
        }
    }

    /// <summary>
    /// Détruit l'ancien visuel et instancie le nouveau pour le stade actuel.
    /// </summary>
    private void UpdateVisual()
    {
        if (_seedData == null || _seedData.stageVisuals == null || _seedData.stageVisuals.Length == 0) return;

        // 1. Détruire l'ancien visuel
        if (_currentVisualInstance != null)
        {
            Destroy(_currentVisualInstance);
        }

        // 2. Instancier le nouveau visuel
        GameObject stageVisualPrefab = _seedData.stageVisuals[_currentStage];
        if (stageVisualPrefab != null && visualContainer != null)
        {
            _currentVisualInstance = Instantiate(stageVisualPrefab, visualContainer);
            // On s'assure qu'il est bien centré localement
            _currentVisualInstance.transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogError($"Crop: Visuel pour le stade {_currentStage} est manquant dans la graine {_seedData.name}.");
        }
    }

    // Fonction à appeler lors d'un clic sur la plante mûre
    public void Harvest()
    {
        if (_actualGrowthTime >= _seedData.GrowthTimeTotal)
        {
            Debug.Log($"Récolte de {_seedData.VegetableProductData.name} !");
            // Logique d'ajout à l'inventaire ici

            Destroy(gameObject); // Supprimer la plante
        }
        else
        {
            Debug.Log("La plante n'est pas encore mûre.");
        }
    }
}