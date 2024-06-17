using UnityEngine;

public partial class PlayerData
{
    [SerializeField] private bool musicState = true;
    [SerializeField] private bool soundState = true;
    [SerializeField] private bool vibrationState = true;

    public bool MusicState
    {
        get => musicState;
        set
        {
            musicState = value;
            Observer.MusicChanged?.Invoke();
        }
    }

    public bool SoundState
    {
        get => soundState;
        set
        {
            soundState = value;
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
