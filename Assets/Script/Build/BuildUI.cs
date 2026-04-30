using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildUI : MonoBehaviour
{
    [SerializeField] Build_Selection build_Selection;

    void Update()
    {
        SelectConstructXbox();
    }
    private void SelectConstructXbox()
    {
        /*
        float inputX = Input.GetAxis("SongX_Xbox");
        float inputY = Input.GetAxis("SongY_Xbox");

        if (Mathf.Abs(inputX) > 0.5f || Mathf.Abs(inputY) > 0.5f)
        {
            int buildIndex = 0;

            if (inputY > 0.5f && Mathf.Abs(inputX) < 0.5f && Input.GetButtonDown("Fire1")) { buildIndex = 4; }
            else if (inputX > 0.3f && inputY > 0.3f) { buildIndex = 3; }
            else if (inputX > 0.5f && Mathf.Abs(inputY) < 0.5f) { buildIndex = 2; }
            else if (inputX > 0.3f && inputY < -0.3f) { buildIndex = 1; }
            else if (inputY < -0.5f && Mathf.Abs(inputX) < 0.5f) { buildIndex = 0; }
            else if (inputX < -0.3f && inputY < -0.3f) { buildIndex = 7; }
            else if (inputX < -0.5f && Mathf.Abs(inputY) < 0.5f) { buildIndex = 6; }
            else if (inputX < -0.3f && inputY > 0.3f) { buildIndex = 5; }

            build_Selection.ConstructChoosen(buildIndex);
        }
        */
    }
} 
