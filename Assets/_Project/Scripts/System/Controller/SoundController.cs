using UnityEngine;

public class SoundController : SingletonDontDestroy<SoundController>
{
    public AudioSource backgroundAudio;
    public AudioSource fxAudio;
    
    [SerializeField] private SoundConfig soundConfig;
    
    public void Start()
    {
        Setup();

        Observer.MusicChanged += OnMusicChanged;
        Observer.SoundChanged += OnSoundChanged;
    }

    private void OnMusicChanged()
    {
        backgroundAudio.mute = !Data.PlayerData.MusicState;
    }
    
    private void OnSoundChanged()
    {
        fxAudio.mute = !Data.PlayerData.SoundState;
    }

    private void Setup()
    {
        OnMusicChanged();
        OnSoundChanged();
    }

    public void PlayFX(SoundName soundName)
    {
        SoundData soundPlayerData = soundConfig.GetSoundDataByType(soundName);

        if (soundPlayerData != null)
        {
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
