using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class MailInputManager : MonoBehaviour
{
    public TMP_InputField emailInput;

    public void OnSubmitEmail()
    {
        string userEmail = emailInput.text;

        if (ValidateEmail(userEmail))
        {
            Debug.Log("Email valide enregistré : " + userEmail);

        }
        else
        {
            Debug.LogWarning("Format d'email invalide !");
        }
    }

    private bool ValidateEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}