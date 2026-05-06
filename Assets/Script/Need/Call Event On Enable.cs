using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CallEventOnEnable : MonoBehaviour
{
    [SerializeField] UnityEvent events;
    private void OnEnable()
    {
        events.Invoke();
    }
}
