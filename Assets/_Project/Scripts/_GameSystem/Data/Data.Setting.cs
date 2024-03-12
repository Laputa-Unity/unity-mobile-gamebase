public static partial class Data
{
    public static bool BgSoundState
    {
        get => GetBool(Constant.BackgroundSoundState, true);
        set
        {
            SetBool(Constant.BackgroundSoundState, value);
            Observer.MusicChanged?.Invoke();
        }
    }

    public static bool FxSoundState
    {
        get => GetBool(Constant.FXSoundState, true);
        set
        {
            SetBool(Constant.FXSoundState, value);
            Observer.SoundChanged?.Invoke();
        }
    }

    public static bool VibrateState
    {
        get => GetBool(Constant.VibrateState, true);
        set
        {
            SetBool(Constant.VibrateState, value);
            Observer.VibrationChanged?.Invoke();
        }
    }
}

public static partial class Constant
{
    public const string BackgroundSoundState = "BACKGROUND_SOUND_STATE";
    public const string FXSoundState = "FX_SOUND_STATE";
    public const string VibrateState = "VIBRATE_STATE";
}