using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Menu : MonoBehaviour
{
    [SerializeField] UnityEvent menuSystem;
    [SerializeField] UnityEvent menuClose;
    bool Open = false;
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !Open)
        {
                Open = !Open;
                menuSystem.Invoke();
        }
        else if (Input.GetButtonDown("Cancel") && Open)
        {
            Open = !Open;
            menuClose.Invoke();
        }
    }
}
