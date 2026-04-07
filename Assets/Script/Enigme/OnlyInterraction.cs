using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnlyInterraction : Interractable
{
    [SerializeField] UnityEvent OnInterract;
    [SerializeField] bool RepeteEvent;
    bool activate = false;
    public override void Interract()
    {
        Debug.Log("test");
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

    public void DebugFunction()
    {
        Debug.Log(" Interraction ");
    }
}
