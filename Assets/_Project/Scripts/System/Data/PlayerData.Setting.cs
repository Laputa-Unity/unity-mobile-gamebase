using UnityEngine;

public partial class PlayerData
{
    [SerializeField] private bool musicState = true;
    [SerializeField] bool soundState = true;
    [SerializeField] bool vibrationState = true;

    public bool MusicState
    {
        get => musicState;
        set => musicState = value;
    }

    public bool SoundState
    {
        get => soundState;
        set => soundState = value;
    }

    public bool VibrationState
    {
        get => vibrationState;
        set => vibrationState = value;
    }
}
