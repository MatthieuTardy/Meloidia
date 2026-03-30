using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundManager : MonoBehaviour
{
    public FMOD.Studio.EventInstance testSFX;

    FMOD.Studio.Bus Master;
    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus SFX;

    [Range(0f, 1f)]
    [SerializeField] float MasterMix = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] float MusicMix = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] float SFXMix = 0.5f;
    // Start is called before the first frame update
    void Awake()
    {
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
        testSFX = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/UI/UI_Selection");
    }

    // Update is called once per frame
    void Update()
    {
        Master.setVolume(MasterMix);
        Music.setVolume(MusicMix);
        SFX.setVolume(SFXMix);
        
    }

    public void MasterVolumeLevel(float Volumelevel)
    {
        MasterMix = Volumelevel;
    }
    public void MusicVolumeLevel(float MusicVolumelevel)
    {
        MusicMix = MusicVolumelevel;
    }
    public void SFXVolumeLevel(float SFXVolumelevel)
    {
        SFXMix = SFXVolumelevel;

        FMOD.Studio.PLAYBACK_STATE PbState;
        testSFX.getPlaybackState(out PbState);
        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            testSFX.start();
        }
    }
    public void SFXPlay()
    {

        FMOD.Studio.PLAYBACK_STATE PbState;
        testSFX.getPlaybackState(out PbState);
        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            testSFX.start();
        }
    }
}
