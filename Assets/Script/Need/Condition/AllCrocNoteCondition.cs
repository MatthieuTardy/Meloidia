using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class AllCrocNoteCondition : Condition
{
    [SerializeField] bool NeedBonheur;
    [ShowIf("NeedBonheur")][SerializeField] [Range(0, 100)] int BonheurNeeded;
    [SerializeField] CrocNoteAmount[] CrocNotesNeeded;

    public override bool CheckCondition()
    {
        Debug.Log(GameManager.Instance.legumeManagerList.Count);
        if (GameManager.Instance.legumeManagerList.Count > 0)
        {
            CrocNoteAmount[] temp = CurrentCrocAmount();
            foreach (var amount in CrocNotesNeeded)
            {
                foreach (var all in temp)
                {
                    if (amount.type == all.type)
                    {
                        if (all.amount >= amount.amount)
                        {

                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }

     
    
    CrocNoteAmount[] CurrentCrocAmount()
    {
        CrocNoteAmount[] temps = new CrocNoteAmount[Enum.GetValues(typeof(CrocNoteType)).Length];
        foreach (int enumValue in Enum.GetValues(typeof(CrocNoteType)))
        {

            temps[enumValue - 1] = new CrocNoteAmount
            {
                amount = 0,
                type = (CrocNoteType)enumValue
            };
        }

        foreach (LegumeManager prox in GameManager.Instance.legumeManagerList)
        {
            if (NeedBonheur)
            {
                if(prox.GetBonheur() >= BonheurNeeded)
                {
                    temps[(int)prox.legumeType - 1].amount++;
                }
            }
            else
            {
                temps[(int)prox.legumeType - 1].amount++;

            }
        }
        return temps;
    }
}
