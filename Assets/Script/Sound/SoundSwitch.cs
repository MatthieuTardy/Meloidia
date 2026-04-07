using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSwitch : MonoBehaviour
{
    public StudioEventEmitter SFXtoOut;
    public StudioEventEmitter SFXtoIn;

    IEnumerator SwitchSound(float time, bool FadeIn)
    {
        int step = 5;
        float volume = 0f;
        float ratio = 1 / step;

        for (int i = 0; i < step; i++)
        {

            FMOD.RESULT result = SFXtoIn.EventInstance.setVolume(volume);
            FMOD.RESULT result2 = SFXtoOut.EventInstance.setVolume(volume);

            yield return new WaitForSeconds(time / step);

        }
    }
}
