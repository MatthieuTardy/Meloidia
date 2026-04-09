using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanchRessourcesGenerator : MonoBehaviour
{
    List<LegumeManager>[] CNbyType = new List<LegumeManager>[5]; //carotte, navet, poivron, chou, brocoli

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



}
