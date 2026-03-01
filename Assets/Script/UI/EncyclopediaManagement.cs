using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncyclopediaManagement : MonoBehaviour
{
    [SerializeField] GameObject[] MelodieContents;
    [SerializeField] GameObject[] CrocNotesContents;
    [SerializeField] GameObject[] RessoucesContents;

    [SerializeField] GameObject[] MainHolder;

    public void SelectMelodieContent(GameObject SelectedContent)
    {
        foreach (var item in MelodieContents)
        {
            if (item == SelectedContent)
            {
                item.SetActive(true);
            }
            else
            {
                item.SetActive(false);
            }
        }
    }

    public void SelectCrocNotesContent(GameObject SelectedContent)
    {
        foreach (var item in CrocNotesContents)
        {
            if (item == SelectedContent)
            {
                item.SetActive(true);
            }
            else
            {
                item.SetActive(false);
            }
        }
    }

    public void SelectRessourcesContent(GameObject SelectedContent) 
    {
        foreach (var item in RessoucesContents)
        {
            if (item == SelectedContent)
            {
                item.SetActive(true);
            }
            else
            {
                item.SetActive(false);
            }
        }
    }

    public void ChangeHolder(GameObject newHolder)
    {
        foreach (var item in MainHolder)
        {
            if (item == newHolder)
            {
                item.SetActive(true);
            }
            else
            {
                item.SetActive(false);
            }
        }
    }

}
