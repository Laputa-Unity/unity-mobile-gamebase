using System;
using System.Collections.Generic;
using CustomTween;
using UnityEngine;

public class SoundController : SingletonDontDestroy<SoundController>
{
    public AudioSource backgroundAudio;
    public AudioSource fxAudio;
    
    [SerializeField] private SoundConfig soundConfig;

    private List<SoundName> _cacheDelaySounds = new List<SoundName>();
    public void Start()
    {
        Setup();

        Observer.MusicChanged += OnMusicChanged;
        Observer.SoundChanged += OnSoundChanged;
    }

    private void OnDestroy()
    {
        Observer.MusicChanged -= OnMusicChanged;
        Observer.SoundChanged -= OnSoundChanged;
    }

    private void OnMusicChanged()
    {
        backgroundAudio.volume = Data.PlayerData.MusicVolume / 3f;
    }
    
    private void OnSoundChanged()
    {
        fxAudio.volume = Data.PlayerData.SoundVolume;
    }

    private void Setup()
    {
        OnMusicChanged();
        OnSoundChanged();
    }

    public SoundData GetSoundData(SoundName soundName)
    {
        return soundConfig.GetSoundDataByType(soundName);
    }

    public void PlayFX(SoundName soundName)
    {
        SoundData soundPlayerData = GetSoundData(soundName);

        if (soundPlayerData != null)
        {
            if (soundPlayerData.delayTime > 0)
            {
                if (!_cacheDelaySounds.Contains(soundName))
                {
                    _cacheDelaySounds.Add(soundName);
                    Tween.Delay(transform, soundPlayerData.delayTime, () => _cacheDelaySounds.Remove(soundName));
                }
                else
                {
                    return;
                }
            }
            
            var soundClip = soundPlayerData.GetRandomAudioClip();
            if (soundClip)
            {
                fxAudio.PlayOneShot(soundClip);
            }
            else
            {
                Debug.LogWarning($"<color=Red>Missing {soundName} clip</color>");
            }
        }
        else
        {
            Debug.LogWarning($"<color=Red>Missing {soundName}</color>");
        }
    }

    public void PlayBackground(SoundName soundName)
    {
        SoundData soundPlayerData = soundConfig.GetSoundDataByType(soundName);

        if (soundPlayerData != null)
        {
            var clip = soundPlayerData.GetRandomAudioClip();

            if (clip)
            {
                if (backgroundAudio.clip != clip)
                {
                    backgroundAudio.clip = clip;
                    backgroundAudio.Play();
                }
            }
            else
            {
                Debug.LogWarning($"<color=Red>Missing {soundName} clip</color>");
            }
        }
        else
        {
            Debug.LogWarning($"<color=Red>Missing {soundName}</color>");
        }
    }

    public void ChangeBackgroundVolume(float volume)
    {
        backgroundAudio.volume = volume;
    }
}
