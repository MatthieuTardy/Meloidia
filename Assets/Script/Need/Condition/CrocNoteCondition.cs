using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CrocNoteCondition : Condition
{
    [SerializeField] CrocNoteProximity proximity;
    [SerializeField] bool haveType;
    [HideIf("haveType")][SerializeField] int WantedCount; 
    [ShowIf("haveType")][SerializeField] CrocNoteAmount[] amounts;
    public override bool CheckCondition()
    {
        if (!haveType)
        {
            if (proximity.CrocNoteInProximity.Count >= WantedCount)
            {
                return true;
            }
        }
        else
        {
            if (proximity.CrocNoteInProximity.Count > 0)
            {
                CrocNoteAmount[] temp = CurrentCrocAmount();
                foreach (var amount in amounts)
                {
                    foreach (var prox in temp)
                    {
                        if (amount.type == prox.type)
                        {
                            if(prox.amount >= amount.amount)
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
        }
            return false;
    }

    CrocNoteAmount[] CurrentCrocAmount()
    {
        CrocNoteAmount[] temps = new CrocNoteAmount[Enum.GetValues(typeof(CrocNoteType)).Length];
        foreach (int enumValue in Enum.GetValues(typeof(CrocNoteType))) 
        {
            Debug.Log("enum value " + enumValue);
            Debug.Log("temp" + temps.Length);
            temps[enumValue - 1] = new CrocNoteAmount
            {
                amount = 0,
                type = (CrocNoteType)enumValue
            };
            Debug.Log("temp [enumvalue]" + temps[enumValue - 1].type);
        }

        foreach (LegumeManager prox in proximity.CrocNoteInProximity)
        {
            temps[(int)prox.legumeType - 1].amount++;
        }
        return temps;
    }
    




}



[System.Serializable]
public class CrocNoteAmount
{
    public CrocNoteType type;
    public int amount;
}
