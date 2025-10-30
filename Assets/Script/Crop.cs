using UnityEngine;
using System.Collections;

public class Crop : MonoBehaviour
{
    private Seed _seedData;
    private int _actualGrowthTime = 0;
    private int _currentStage = 0;
    private bool isHarvesting = false;

    [Header("Référence Visuelle")]
    [Tooltip("Le conteneur des visuels (enfant de ce GameObject) qui sera mis à jour.")]
    public Transform visualContainer;

    private GameObject _currentVisualInstance;

    public System.Action<Crop> OnHarvestedAndDestroyed;

    public void Initialize(Seed seed)
    {
        _seedData = seed;
        _actualGrowthTime = 0;
        _currentStage = 0;

        if (visualContainer == null)
        {
            visualContainer = transform; // Si non assigné, on utilise le transform du Crop lui-même.
            Debug.LogWarning("Crop: visualContainer n'est pas assigné. Utilisation du Transform du prefab Crop par défaut.");
        }

        UpdateVisual();
    }

    public void TickGrowth(int timeIncrease = 1)
    {
        if (_seedData == null || _actualGrowthTime >= _seedData.GrowthTimeTotal || isHarvesting)
            return;

        _actualGrowthTime += timeIncrease;
        int newStage = _seedData.CalculateCurrentStage(_actualGrowthTime);

        if (newStage != _currentStage)
        {
            _currentStage = newStage;
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        if (_seedData == null || _seedData.stageVisuals == null || _seedData.stageVisuals.Length == 0) return;

        if (_currentVisualInstance != null)
        {
            Destroy(_currentVisualInstance);
        }

        GameObject stageVisualPrefab = _seedData.stageVisuals[_currentStage];
        if (stageVisualPrefab != null)
        {
            _currentVisualInstance = Instantiate(stageVisualPrefab, visualContainer);
            _currentVisualInstance.transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogError($"Crop: Visuel pour le stade {_currentStage} est manquant dans la graine {_seedData.name}.");
        }
    }

    public void Harvest()
    {
        if (_actualGrowthTime >= _seedData.GrowthTimeTotal && !isHarvesting)
        {
            isHarvesting = true;
            Debug.Log($"Récolte de {_seedData.VegetableProductData.name} !");
            // Logique d'inventaire ici

            // On s'assure qu'il y a bien un visuel à animer
            if (_currentVisualInstance != null)
            {
                StartCoroutine(ShrinkAndDestroy());
            }
            else
            {
                // S'il n'y a pas de visuel, on détruit directement pour ne pas laisser un objet vide
                FinalizeDestruction();
            }
        }
    }

    private IEnumerator ShrinkAndDestroy()
    {
        float duration = 0.5f;
        // On cible directement le Transform de l'instance du visuel
        Transform visualTransform = _currentVisualInstance.transform;
        Vector3 initialScale = visualTransform.localScale;
        Vector3 targetScale = Vector3.zero;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            visualTransform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        visualTransform.localScale = targetScale;

        // On finalise la destruction
        FinalizeDestruction();
    }

    private void FinalizeDestruction()
    {
        // Notifier le manager que la plante est prête à être retirée du dictionnaire
        OnHarvestedAndDestroyed?.Invoke(this);

        // Détruire l'objet racine de la plante (le prefab Crop)
        Destroy(gameObject);
    }
}