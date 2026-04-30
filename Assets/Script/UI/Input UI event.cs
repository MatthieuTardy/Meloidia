using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUIevent : MonoBehaviour
{
    public static InputUIevent instance;

    public event Action IsSinging;
    bool isSinging;
    bool SingWheelActive;

    public event Action InOption;
    public bool isInOption;

    public event Action InOtherUI;
    public bool isInOtherUI;

    private void Start()
    {
        if (instance != null) 
        {
            Destroy(instance);
        }
        instance = this;
        _UI = null;
        isSinging = false;
        SingWheelActive = false;
        isInOption = false;
        isInOtherUI = false;
    }
    void Update()
    {
        ManageEscape();
        ManageSinging();
    }

    void ManageSinging()
    {
        if (!isInOption && !isInOtherUI)
        {
            float value = Input.GetAxisRaw("ToggleSing");
            value += Input.GetAxisRaw("SongPC");
            if (value > 0 && !isSinging)
            {
                Debug.Log("Invoke");
                isSinging = true;
                SingWheelActive = !SingWheelActive;
                IsSinging?.Invoke();
            }
            else if (value <= 0 && isSinging)
            {
                isSinging = false;
            }
        }
    }
    void ManageEscape()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (!isInOtherUI)
            {
                if (SingWheelActive)
                {
                    isSinging = !isSinging;
                    SingWheelActive = !SingWheelActive;
                    IsSinging.Invoke();
                    
                }
                else
                {
                    isInOption = !isInOption;
                    InOption?.Invoke();
                }
            }
            else
            {
                _UI.SetActive(false);
                _UI = null;
                isInOtherUI = false;
                HideCursor();
            }
        }
    }

    GameObject _UI = null;
    public void OtherUIAction(GameObject ui, bool showCursor)
    {
        _UI = ui;
        if (SingWheelActive)
        {
            isSinging = !isSinging;
            SingWheelActive = !SingWheelActive;
            IsSinging.Invoke();
        }
        isInOtherUI = true;
        _UI.SetActive(true);
        if (showCursor) 
        {
            ShowCursor();
        }
    }

    public void ExitOptionUI()
    {
        isInOption = !isInOption;
        InOption?.Invoke();
        HideCursor() ;
    }

    public static void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public static void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ExitOtherUI()
    {
        _UI.SetActive(false);
        _UI = null;
        isInOtherUI = false;
        HideCursor();
    }
}
