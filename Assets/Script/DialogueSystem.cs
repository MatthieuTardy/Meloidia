using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] Dialogue[] Dialogues;
    [SerializeField] UnityEvent[] OnDialoguesFinish;

    [Header("Settings")]
    [SerializeField] GameObject DialoguePrefab;
    //[SerializeField] TreeNeed need;
    
    [SerializeField] float textSpeed = 0.04f; 

    int index;
    bool isActive;
    bool isTyping; 
    bool canSkip; // NOUVEAU : Empêche le skip instantané au lancement

    GameObject currentDialogueObject;
    TextMeshProUGUI textComponent;
    Coroutine typingCoroutine;
    
    public void Interract()
    {
       // if(need != null)
        {
           // need.CheckNeed();
        }
        if (!isActive)
        {
            Debug.Log("Interact");
            isActive = true;
            LaunchDialogue();
        }
    }

    void LaunchDialogue()
    {
        currentDialogueObject = Instantiate(DialoguePrefab, transform);
        textComponent = currentDialogueObject.GetComponentInChildren<TextMeshProUGUI>();
        
        Dialogues[index].CurrentDialogue = 0;
        
        // IMPORTANT : On désactive le skip temporairement
        canSkip = false;
        StartCoroutine(EnableSkipRoutine());

        StartTyping(Dialogues[index].dialogue[0]);
    }

    // Attend une frame pour s'assurer que l'input d'interaction n'est pas compté comme un skip
    IEnumerator EnableSkipRoutine()
    {
        yield return null; 
        canSkip = true;
    }

    void Update()
    {
        // On ajoute la vérification !canSkip
        if (!isActive || !canSkip) return;

        if (Input.GetButtonDown("Fire1"))
        {
            if (isTyping)
            {
                CompleteTextImmediately();
            }
            else
            {
                NextLine();
            }
        }
    }

    void StartTyping(string line)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        textComponent.text = ""; 

        foreach (char letter in line.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    void CompleteTextImmediately()
    {
        StopCoroutine(typingCoroutine);
        textComponent.text = Dialogues[index].dialogue[Dialogues[index].CurrentDialogue];
        isTyping = false;
    }

    void NextLine()
    {
        if (Dialogues[index].CurrentDialogue < Dialogues[index].dialogue.Length - 1)
        {
            Dialogues[index].CurrentDialogue++;
            StartTyping(Dialogues[index].dialogue[Dialogues[index].CurrentDialogue]);
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        Debug.Log("End Dialogue");
        if (OnDialoguesFinish.Length > index)
        {
            OnDialoguesFinish[index].Invoke();
        }
        DesactivateUI();
    }

    public void Setindex(int newIndex)
    {
        index = newIndex;
    }

    public void DesactivateUI()
    {
        if (currentDialogueObject != null)
        {
            Destroy(currentDialogueObject);
        }
        isActive = false;
        isTyping = false;
        canSkip = false;
    }
}

[Serializable]
public class Dialogue
{
    [TextArea(3, 10)] public string[] dialogue;
    public int CurrentDialogue;
}