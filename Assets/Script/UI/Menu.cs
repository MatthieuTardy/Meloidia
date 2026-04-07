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

    float DefaultTimeScale;
    private void Start()
    {
        DefaultTimeScale = Time.timeScale;    
        Debug.Log(DefaultTimeScale);
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

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Quitter()
    {
        Application.Quit();
    }
}
