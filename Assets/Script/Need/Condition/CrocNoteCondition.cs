using System.Collections;
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
            if(proximity.CrocNoteInProximity.Count >= WantedCount)
            {
                return true;
            }
        }
        else
        {
            if(proximity.CrocNoteInProximity.Count > 0)
            {
                CrocNoteAmount[] temp = CurrentCrocAmount();
            }
        }
            return false;
    }

    CrocNoteAmount[] CurrentCrocAmount()
    {
        CrocNoteAmount[] temp = new CrocNoteAmount[amounts.Length];
        foreach(var A in proximity.CrocNoteInProximity)
        {

        }
        return null;
    }




}



[System.Serializable]
public class CrocNoteAmount
{
    public CrocNoteType type;
    public int amount;
}
