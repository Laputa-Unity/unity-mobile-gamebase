using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : Singleton<SoundController>
{
    public AudioSource BackgroundAudio;
    public AudioSource FxAudio;
    public List<AudioClip> AudioClips = new List<AudioClip>();

    private void Start()
    {
        DontDestroyOnLoad(this);
        Init();
    }

    private void Init()
    {
        BackgroundAudio.loop = true;

        for (int i = 0; i < Enum.GetNames(typeof(SoundType)).Length; i++)
        {
            SoundData soundData = ConfigController.Sound.SoundDatas.Find(item => item.SoundType == (SoundType) i);
            AudioClips.Add(soundData.Clip);
        }
    }

    public void PlayFX(SoundType soundType)
    {
        AudioClip clip = AudioClips[(int)soundType];

        if (!clip || !Data.SoundState) return;

        FxAudio.PlayOneShot(clip);
    }

    public void PlayBackground(SoundType soundType)
    {
        AudioClip clip = AudioClips[(int)soundType];

        if (!clip || !Data.MusicState) return;
        
        BackgroundAudio.clip = clip;
        BackgroundAudio.Play();
    }

    public void PauseBackground()
    {
        if (BackgroundAudio)
        {
            BackgroundAudio.Pause();
        }
    }

    public AudioSource PlayLoop(SoundType soundType)
    {
        AudioClip clip = AudioClips[(int)soundType];

        if (!clip || !Data.SoundState) return null;

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.playOnAwake = true;
        audioSource.loop = true;
        audioSource.Play();

        return audioSource;
    }
}
