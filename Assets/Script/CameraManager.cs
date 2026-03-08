using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
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
        if (value > 0 && !triggerPressed)
        {
            triggerPressed = true;
            IsToggleSing = !IsToggleSing;
        }

        if (value < 0.1f)
        {
            triggerPressed = false;
        }
    }


    //LA FONCTION SE LANCE QUAND LE JOUEUR N'EST PAS EN CHANT
    public void CameraFunction()
    {

    }
}
