using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolUIManager : MonoBehaviour
{
    [Header("Références")]
    public PlayerManager playerManager;

    [Header("UI pour l'outil Pelle (outils = 2)")]
    public GameObject pelleUiContainer;
    public GameObject pelleUiTerrePleine; // Image si terre > 0
    public GameObject pelleUiTerreVide;   // Image si terre <= 0

    [Header("UI pour l'outil Arrosoir (outils = 1)")]
    public GameObject arrosoirUiContainer;
    public GameObject arrosoirUiEauPleine; // Image si eau > 0
    public GameObject arrosoirUiEauVide;   // Image si eau <= 0


    [Header("UI pour Essence Magique")]
    public TextMeshProUGUI essenceText;

    void Start()
    {
        if (pelleUiContainer != null) pelleUiContainer.SetActive(false);
        if (arrosoirUiContainer != null) arrosoirUiContainer.SetActive(false);
        if (playerManager == null)
        {
            playerManager = FindObjectOfType<PlayerManager>();
        }
    }

    void Update()
    {
        if (playerManager == null)
        {
            Debug.LogWarning("Le PlayerManager n'est pas assigné dans le ToolUIManager !");
            return;
        }
        if (playerManager != null && essenceText != null)
        {
            // Affiche uniquement le nombre actuel
            essenceText.text = "" + playerManager.essenceMagique;
        }

        // --- GESTION DE L'UI POUR LA PELLE (outils = 2) ---
        if (playerManager.outils == 2)
        {
            if (pelleUiContainer != null) pelleUiContainer.SetActive(true);

            // CHANGEMENT : On vérifie s'il reste de la terre.
            if (playerManager.GetDirt() > 0)
            {
                if (pelleUiTerrePleine != null) pelleUiTerrePleine.SetActive(true);
                if (pelleUiTerreVide != null) pelleUiTerreVide.SetActive(false);
            }
            else // S'il n'y a plus de terre
            {
                if (pelleUiTerrePleine != null) pelleUiTerrePleine.SetActive(false);
                if (pelleUiTerreVide != null) pelleUiTerreVide.SetActive(true);
            }
        }
        else
        {
            if (pelleUiContainer != null) pelleUiContainer.SetActive(false);
        }

        // --- GESTION DE L'UI POUR L'ARROSOIR (outils = 1) ---
        if (playerManager.outils == 1)
        {
            if (arrosoirUiContainer != null) arrosoirUiContainer.SetActive(true);

            // CHANGEMENT : On vérifie s'il reste de l'eau.
            if (playerManager.GetWater() > 0)
            {
                if (arrosoirUiEauPleine != null) arrosoirUiEauPleine.SetActive(true);
                if (arrosoirUiEauVide != null) arrosoirUiEauVide.SetActive(false);
            }
            else // S'il n'y a plus d'eau
            {
                if (arrosoirUiEauPleine != null) arrosoirUiEauPleine.SetActive(false);
                if (arrosoirUiEauVide != null) arrosoirUiEauVide.SetActive(true);
            }
        }
        else
        {
            if (arrosoirUiContainer != null) arrosoirUiContainer.SetActive(false);
        }
    }
}