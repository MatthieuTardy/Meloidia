using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : MonoBehaviour
{
    public bool ConditionMet;
    public abstract bool CheckCondition();
}
