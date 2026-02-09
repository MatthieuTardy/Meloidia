using System;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // Requis pour détecter la sélection

public class MailInputManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    private string fileName = "emails_collectes.txt";

    void Start()
    {
        // On ajoute dynamiquement les écouteurs pour la pause
        emailInput.onSelect.AddListener(delegate { PauseGame(true); });
        emailInput.onDeselect.AddListener(delegate { PauseGame(false); });
    }

    public void OnSubmitEmail()
    {
        string userEmail = emailInput.text;

        if (ValidateEmail(userEmail))
        {
            SaveEmailToPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), userEmail, "Bureau");
            SaveEmailToPath(Application.persistentDataPath, userEmail, "PersistentData");

            emailInput.text = "";
            // Désélectionne le champ après l'envoi pour relancer le jeu
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Debug.LogWarning("Format d'email invalide !");
        }
    }

    private void PauseGame(bool isPaused)
    {
        // Si isPaused est vrai, le temps s'arrête (0), sinon il reprend (1)
        Time.timeScale = isPaused ? 0f : 1f;


        Debug.Log(isPaused ? "Jeu en PAUSE (Saisie mail...)" : "Jeu en REPRISE");
    }

    private void SaveEmailToPath(string folderPath, string email, string label)
    {
        try
        {
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, fileName);

            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} : {email}");
            }
            Debug.Log($"[{label}] Succès : {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[{label}] Erreur : {e.Message}");
        }
    }

    private bool ValidateEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}