using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] Dialogue[] Dialogues;
    [SerializeField] UnityEvent[] OnDialoguesFinish;

    int index;
    bool isActive;

    [SerializeField] GameObject DialoguePrefab;
    GameObject currentDialogueObject;
    TextMeshProUGUI text;
    
    public void Interract()
    {
        if (!isActive)
        {
            Debug.Log("interract");
            isActive = true;
            LaunchDialogue();
        }
    }

    void LaunchDialogue()
    {
        currentDialogueObject = Instantiate(DialoguePrefab, transform);
        text = currentDialogueObject.GetComponentInChildren<TextMeshProUGUI>();
        Dialogues[index].CurrentDialogue = 0;
        ChangeText(0);
        StartCoroutine(SkipDialogue());
    }

    void ChangeText(int newDialogue)
    {
        text.text = Dialogues[index].dialogue[newDialogue];
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
    }
    IEnumerator SkipDialogue()
    {
        yield return null;
        yield return new WaitUntil(() => Input.GetButtonDown("Fire1"));

        if (Dialogues[index].CurrentDialogue < Dialogues[index].dialogue.Length-1)
        {
            Dialogues[index].CurrentDialogue++;
            ChangeText(Dialogues[index].CurrentDialogue);
            yield return null;
            StartCoroutine(SkipDialogue());
        }
        else
        {
            Debug.Log("End Dialogue");
            OnDialoguesFinish[index].Invoke();
        }

    }
}


[Serializable]
public class Dialogue
{
    [TextArea]public string[] dialogue;
    public bool isFinish;
    public int CurrentDialogue;
}
