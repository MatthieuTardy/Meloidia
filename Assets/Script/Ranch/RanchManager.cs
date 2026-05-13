using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanchManager : MonoBehaviour
{
    // si des crocs Notes sont dans le ranch
    // on les ajoutes dans une listes -> liste utilisé par les conditions

    public List<LegumeManager> CrocNotesInRanch;
    public List<LegumeManager>[] CNbyType = new List<LegumeManager>[5]; //carotte, navet, poivron, chou, brocoli
    #region default function
    private void Start()
    {
        CrocNotesInRanch = new List<LegumeManager>();
        for (int i = 0; i < CNbyType.Length; i++) { CNbyType[i] = new List<LegumeManager>(); }

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 7)//legume
        {
            LegumeManager CN = other.GetComponent<LegumeManager>();
            CN.Rename();
            AddingCrocNote(CN);
        }
    }

    #endregion
    #region manage list
    void AddingCrocNote(LegumeManager CN)
    {
        if (!CrocNotesInRanch.Contains(CN))
        {

            CrocNotesInRanch.Add(CN);
            UpdateList(CN);
        }
    }
    public void UpdateList(LegumeManager CN)
    {
        if (CN.legumeType == CrocNoteType.un)
        {
            CNbyType[0].Add(CN);
        }
        else if (CN.legumeType == CrocNoteType.deux)
        {
            CNbyType[1].Add(CN);
        }
        else if (CN.legumeType == CrocNoteType.trois)
        {
            CNbyType[2].Add(CN);
        }
        else if (CN.legumeType == CrocNoteType.quatre)
        {
            CNbyType[3].Add(CN);
        }
        else if (CN.legumeType == CrocNoteType.cinq)
        {
            CNbyType[4].Add(CN);
        }
    }
    #endregion

    public int getCNNumberByType(CrocNoteType type)
    {
        switch (type)
        {
            case CrocNoteType.un:
                return CNbyType[0].Count;
            case CrocNoteType.deux:
                return CNbyType[1].Count;
            case CrocNoteType.trois:
                return CNbyType[2].Count;
            case CrocNoteType.quatre:
                return CNbyType[3].Count;
            case CrocNoteType.cinq:
                return CNbyType[4].Count;
            default:
                return 0;
        }
    }
}
