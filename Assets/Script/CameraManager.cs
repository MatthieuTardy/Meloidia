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
        Cursor.lockState = CursorLockMode.Confined;
    }
    void Update()
    {
        ManageToggleSing();
        if (!IsToggleSing)
        {
            CameraFunction();
        }

    }

    bool IsToggleSing = false;
    bool triggerPressed;

    public void ManageToggleSing()
    {
        float value = Input.GetAxisRaw("ToggleSing");
        value += Input.GetAxisRaw("SongPC");
        if (value > 0 && !triggerPressed)
        {
            triggerPressed = true;
            IsToggleSing = !IsToggleSing;

            if (IsToggleSing)
            {
                LockCam(false);
            }
            else
            {
                LockCam(true);
            }
        }

        if (value < 0.1f)
        {
            triggerPressed = false;
        }
    }

    void LockCam( bool enable)
    {
        freeLook.enabled = enable;
        Cursor.visible = enable;
        if (enable)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
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
