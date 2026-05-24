using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnlyInterraction : Interractable
{
    [SerializeField] UnityEvent OnInterract;
    [SerializeField] Condition condition;
    [SerializeField] bool RepeteEvent;
    bool activate = false;

    public override void Interract()
    {
        if (condition == null)
        {
            if (RepeteEvent)
            {
                OnInterract.Invoke();
            }
            else if (!activate)
            {
                OnInterract.Invoke();
                activate = true;
            }
        }
        else
        {
            if (condition.CheckCondition())
            {
                if (RepeteEvent)
                {
                    OnInterract.Invoke();
                }
                else if (!activate)
                {
                    OnInterract.Invoke();
                    activate = true;
                }
            }
        }
    }

    public void DebugFunction()
    {
        Debug.Log(" Interraction ");
    }

    public void DestroyTarget(GameObject target)
    {
        if (target != null)
        {
            Destroy(target);
        }
    }
}