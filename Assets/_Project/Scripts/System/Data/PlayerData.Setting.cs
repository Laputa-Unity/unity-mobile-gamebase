using UnityEngine;

public partial class PlayerData
{
    [SerializeField] private float musicVolume = 1;
    [SerializeField] private float soundVolume = 1;
    [SerializeField] private bool vibrationState = true;

    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = value;
            Observer.MusicChanged?.Invoke();
        }
    }

    public float SoundVolume
    {
        get => soundVolume;
        set
        {
            soundVolume = value;
            Observer.SoundChanged?.Invoke();
        }
    }

    public bool VibrationState
    {
        get => vibrationState;
        set
        {
            vibrationState = value;
            Observer.VibrationChanged?.Invoke();
        }
    }
}