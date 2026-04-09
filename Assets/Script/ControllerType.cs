using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ControllerType : MonoBehaviour
{
    public static bool IsUseController;
    [SerializeField] UnityEvent OnController;
    [SerializeField] UnityEvent OnKeyboard;
    private void Update()
    {
        bool useController = IsUseController;

        if (Input.GetAxisRaw("TestController") != 0)
        {
            IsUseController = true;
        }
        else if(Input.GetAxisRaw("TestKeyBoard") != 0)
        {
            IsUseController = false;
        }

        if (useController != IsUseController) 
        {
            if (IsUseController) { OnController.Invoke(); }
            else { OnKeyboard.Invoke(); }
        }
    }
}
