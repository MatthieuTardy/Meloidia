using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSound : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference _footsteps;
    private FMOD.Studio.EventInstance footsteps;
    private void Awake()
    {
        if (!_footsteps.IsNull)
        {
            footsteps = FMODUnity.RuntimeManager.CreateInstance(_footsteps);
            Debug.Log("Init_Step");
        }
    }

    public void PlayerFootStep()
    {
        Debug.Log("Step");
        if (footsteps.isValid())
        {

            footsteps.start();
        }
    }
}
