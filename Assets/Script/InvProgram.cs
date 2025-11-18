using System;
using System.Threading;
using UnityEngine;

public class InvProgram : MonoBehaviour
{
    // Simule la référence aux 4+ éléments UI de la barre d'inventaire
    private static string[] _uiElements;

    public static void Main(string[] args)
    {
        // 1. Initialiser le Manager
        InventaireManager inventory = new InventaireManager(4);

        // 2. Initialiser l'UI et s'abonner aux changements du manager
        InitializeUI(4, inventory);
        inventory.OnSelectionChanged += UpdateUISelection;

        Console.WriteLine("------------------------------------------");
        Console.WriteLine("    Simulateur de Barre d'Inventaire");
        Console.WriteLine("------------------------------------------");

        // Afficher l'état initial
        DisplayInventoryContent(inventory);

        // --- Étape 1 : Navigation dans 4 slots ---

        Console.WriteLine("\nNavigation Manette (4 slots)");
        SimulateInput(inventory, "Droite", 3); // Déplacement de 0 -> 1 -> 2 -> 3
        SimulateInput(inventory, "Droite", 1); // Déplacement de 3 -> 0 (Retour au début)

        // --- Étape 2 : Agrandissement ---

        Console.WriteLine("\n--- 🎁 Agrandissement de la sacoche (+2 slots) ---");
        inventory.ExpandInventory(2); // Taille totale : 6

        // Mettre à jour la simulation d'UI pour les 2 nouveaux emplacements
        UpdateSimulatedUI(6);

        // --- Étape 3 : Navigation dans 6 slots ---

        Console.WriteLine("\n--- 🎮 Navigation Manette (6 slots) ---");
        SimulateInput(inventory, "Gauche", 1); // Déplacement de 0 -> 5 (Retour à la fin)
        SimulateInput(inventory, "Droite", 3); // Déplacement de 5 -> 0 -> 1 -> 2

        DisplayInventoryContent(inventory);
    }

    // --- Fonctions de Simulation UI ---

    private static void InitializeUI(int initialSize, InventaireManager manager)
    {
        _uiElements = new string[initialSize];
        for (int i = 0; i < initialSize; i++)
        {
            // Initialisation de l'affichage de l'UI
            var slot = manager.GetSlot(i);
            _uiElements[i] = $"[Slot {i}: {slot.ItemName ?? "Vide"} ({slot.Quantity})]";
        }
        // Marquer l'élément initial comme sélectionné
        _uiElements[manager.SelectedIndex] = ">> " + _uiElements[manager.SelectedIndex] + " <<";
    }

    private static void UpdateSimulatedUI(int newSize)
    {
        // Crée un nouveau tableau avec la nouvelle taille
        string[] newUI = new string[newSize];

        // Copie les anciens éléments
        Array.Copy(_uiElements, newUI, _uiElements.Length);

        // Initialise les nouveaux éléments
        for (int i = _uiElements.Length; i < newSize; i++)
        {
            newUI[i] = $"[Slot {i}: Vide (0)]";
        }

        _uiElements = newUI;
        Console.WriteLine("[UI] Mise à jour des éléments visuels pour 6 slots.");
    }

    /// <summary>
    /// Fonction appelée par l'événement du Manager pour mettre à jour l'affichage.
    /// </summary>
    private static void UpdateUISelection(int oldIndex, int newIndex)
    {
        // 1. Nettoyer l'ancienne sélection
        string oldElement = _uiElements[oldIndex];
        oldElement = oldElement.Replace(">> ", "").Replace(" <<", ""); // Nettoie les marques
        _uiElements[oldIndex] = oldElement;

        // 2. Marquer la nouvelle sélection
        _uiElements[newIndex] = ">> " + _uiElements[newIndex] + " <<";

        Console.WriteLine($"[UI Update] Sélection déplacée de #{oldIndex} à #{newIndex}");
        Console.WriteLine($"Barre d'inventaire actuelle : {string.Join(" ", _uiElements)}");
    }

    // --- Fonctions d'Exécution ---

    private static void SimulateInput(InventaireManager inventory, string direction, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (direction == "Droite")
            {
                inventory.MoveRight();
            }
            else if (direction == "Gauche")
            {
                inventory.MoveLeft();
            }
            Thread.Sleep(100); // Petite pause pour simuler le temps entre les pressions
        }
    }

    private static void DisplayInventoryContent(InventaireManager inventory)
    {
        Console.WriteLine("\n--- Contenu Final (Vérification) ---");
        for (int i = 0; i < _uiElements.Length; i++)
        {
            var slot = inventory.GetSlot(i);
            string content = slot.IsEmpty ? "VIDE" : $"{slot.ItemName} x{slot.Quantity}";
            string status = i == inventory.SelectedIndex ? " (ACTIF)" : "";
            Console.WriteLine($"- Emplacement #{i}: {content}{status}");
        }
    }
}