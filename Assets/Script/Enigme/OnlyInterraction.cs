using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class OnlyInterraction : MonoBehaviour
{
    [SerializeField] UnityEvent OnInterract;
    [SerializeField] bool RepeteEvent;
    bool activate = false;
    public void Interract()
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
