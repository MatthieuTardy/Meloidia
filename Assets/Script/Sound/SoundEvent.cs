using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SoundEvent : MonoBehaviour
{
    public StudioEventEmitter SFXtoOut;
    public StudioEventEmitter SFXtoIn;
    public UnityEvent FadeEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            FadeEvent.Invoke();
        }




    }
    public void SwitchActiver(float time)
    {
        StartCoroutine(SwitchSound(time));
    }

    public void InActiver(float time)
    {
        StartCoroutine(InSound(time));
    }

    public void OutActiver(float time)
    {
        StartCoroutine(OutSound(time));
    }

    private IEnumerator SwitchSound(float time)
    {
        //Elle fade in et out 2 sons

        //prend les 2 sons

        // je def un volume de base (1 ou 0)

        // puis je change petit à petit avec une boucle for



        int step = 20;

        float ratio = 1f / step;

        if (SFXtoOut.IsPlaying())
        {

            float volumeIn = 0f;
            float volumeOut = 1f;
            SFXtoIn.Play();
            for (int i = 0; i < step-1; i++)
            {
                volumeIn += ratio;
                volumeOut -= ratio;

                FMOD.RESULT result = SFXtoIn.EventInstance.setVolume(volumeIn);

                if (volumeOut <= 0f)
                {
                    FMOD.RESULT result2 = SFXtoOut.EventInstance.setVolume(volumeOut);
                }


                yield return new WaitForSeconds(time / step);

            }
            SFXtoOut.Stop();
        }
    }
    private IEnumerator OutSound(float time)
    {
        int step = 20;

        float ratio = 1f / step;

        if (!SFXtoOut.IsPlaying())
        {
            float volumeOut = 1f;
            for (int i = 0; i < step-1; i++)
            {
                volumeOut -= ratio;
                if (volumeOut <= 0f)
                {
                    FMOD.RESULT result2 = SFXtoOut.EventInstance.setVolume(volumeOut);
                }
                yield return new WaitForSeconds(time / step);
            }
            SFXtoOut.Stop();
        }
    }
    private IEnumerator InSound(float time)
    {
        int step = 20;

        float ratio = 1f / step;

        if (!SFXtoOut.IsPlaying())
        {
            float volumeIn = 0f;
            for (int i = 0; i < step - 1; i++)
            {
                volumeIn += ratio;
                FMOD.RESULT result = SFXtoIn.EventInstance.setVolume(volumeIn);

                yield return new WaitForSeconds(time / step);
            }
            SFXtoOut.Stop();
        }
    }
}
