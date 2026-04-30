using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook freeLook;
    [SerializeField] string MouseY, MouseX;
    [SerializeField] string JoystickY, JoystickX;
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InputUIevent.instance.IsSinging += ManageToggleSing;
    }
    void Update()
    {
        if (!IsToggleSing)
        {
            CameraFunction();
        }
        else
        {
            freeLook.m_YAxis.m_InputAxisName = "";
            freeLook.m_XAxis.m_InputAxisName = "";
        }

    }

    bool IsToggleSing = false;
    bool triggerPressed;

    public void ManageToggleSing()
    {
        bool isPC = Input.GetButtonDown("SongPC");

        IsToggleSing = !IsToggleSing;

        if (IsToggleSing)
        {
            LockCam(true, isPC);
        }
        else
        {
            LockCam(false, isPC);
        }

    }

    void LockCam(bool enable,bool IsPC)
    {
        // freeLook.enabled = enable;
        freeLook.m_XAxis.Reset();
        freeLook.m_YAxis.Reset();
        // GameManager.Instance.playerManager.LockControl(enable);
        InputUIevent.HideCursor();
        if (IsPC)
        {
            if (enable)
            {
                InputUIevent.ShowCursor();
            }
            else
            {
                InputUIevent.HideCursor();
            }
        }
    }

    //LA FONCTION SE LANCE QUAND LE JOUEUR N'EST PAS EN CHANT
    public void CameraFunction()
    {
        //LockCam();
        changeAxis();
    }


    void changeAxis()
    {
        if(Input.GetAxis("SongY_Xbox") != 0 || Input.GetAxis("SongX_Xbox") != 0)
        {
            //Debug.Log("Input controller");
            freeLook.m_YAxis.m_InputAxisName = JoystickY;
            freeLook.m_XAxis.m_InputAxisName = JoystickX;
        }
        else if(Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
           // Debug.Log("Input mouse");
            freeLook.m_YAxis.m_InputAxisName = MouseY;
            freeLook.m_XAxis.m_InputAxisName = MouseX;
        }
    }
}
