using System;

public static class Observer
{
    #region GameSystem
    // Debug
    public static Action DebugChanged;
    // Currency
    public static Action<int> MoneyChanged;
    // Level Spawn
    public static Action CurrentLevelChanged;
    // Setting
    public static Action MusicChanged;
    public static Action SoundChanged;
    public static Action VibrationChanged;
    // Daily Reward
    public static Action ClaimDailyReward;

    // Other
    public static Action CoinMove;
    public static Action PurchaseFail;
    public static Action PurchaseSucceed;
    public static Action ClaimReward;

    #endregion

    #region Gameplay
    // Game event
    public static Action<Level> StartLevel;
    public static Action<Level> ReplayLevel;
    public static Action<Level> SkipLevel;
    public static Action<Level> WinLevel;
    public static Action<Level> LoseLevel;

    public static Action<string> EquipItem;

    #endregion
}
