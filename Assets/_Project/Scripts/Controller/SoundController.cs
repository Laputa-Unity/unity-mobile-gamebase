using UnityEngine;

public class SoundController : SingletonDontDestroy<SoundController>
{
    public AudioSource backgroundAudio;
    public AudioSource fxAudio;
    private SoundConfig SoundConfig => ConfigController.Sound;

    public void Start()
    {
        Setup();

        Observer.MusicChanged += OnMusicChanged;
        Observer.SoundChanged += OnSoundChanged;
    }

    private void OnMusicChanged()
    {
        backgroundAudio.mute = !Data.BgSoundState;
    }
    
    private void OnSoundChanged()
    {
        fxAudio.mute = !Data.FxSoundState;
    }

    private void Setup()
    {
        OnMusicChanged();
        OnSoundChanged();
    }

    public void PlayFX(SoundName soundName)
    {
        SoundData soundData = SoundConfig.GetSoundDataByType(soundName);

        if (soundData != null)
        {
            var soundClip = soundData.GetRandomAudioClip();
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
        SoundData soundData = SoundConfig.GetSoundDataByType(soundName);

        if (soundData != null)
        {
            var clip = soundData.GetRandomAudioClip();

            if (clip)
            {
                if (!backgroundAudio.clip == clip)
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
}
