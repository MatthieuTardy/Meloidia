using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleInterraction : Interractable
{
    bool isToggled;

    [SerializeField] UnityEvent Toggle;
    [SerializeField] UnityEvent Untoggle;
    public override void Interract()
    {
        if (!isToggled)
        {
            isToggled = true;
            Toggle.Invoke();
        }
        else
        {
            isToggled = false;
            Untoggle.Invoke();
        }
    }
}
