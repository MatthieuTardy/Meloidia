using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.Events;


public class Menu : MonoBehaviour
{
    [SerializeField] UnityEvent menuSystem;
    [SerializeField] UnityEvent menuClose;
    bool Open = false;
    float DefaultTimeScale;

    private void Start()
    {
        DefaultTimeScale = Time.timeScale;    
        Debug.Log(DefaultTimeScale);
        InputUIevent input = FindAnyObjectByType<InputUIevent>();
        input.InOption += ManageInput;
    }

    void ManageInput()
    {
        if (!Open)
        {
            Open = !Open;
            menuSystem.Invoke();
            InputUIevent.ShowCursor();
        }
        else
        {
            Open = !Open;
            menuClose.Invoke();
            InputUIevent.HideCursor();
        }
    }
    public void StopTime()
    {
       // Debug.Log(Time.timeScale + "stop");
        Time.timeScale = 0f;
       // Debug.Log(Time.timeScale + "stop");
    }

    public void ResetTime()
    {
       // Debug.Log(Time.timeScale + "Reset");
        Time.timeScale = DefaultTimeScale;
       // Debug.Log(Time.timeScale + "Reset");
    }

    public void Quitter()
    {
        Application.Quit();
    }
}
