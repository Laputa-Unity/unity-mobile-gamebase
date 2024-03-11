using System;
using UnityEngine;

public static partial class Data
{
    #region GAME_DATA
    
    public static bool IsFirstOpenGame
    {
        get => PlayerPrefs.GetInt(Constant.IsFirstOpenGame, 0) == 1;
        set => PlayerPrefs.SetInt(Constant.IsFirstOpenGame, value ? 1 : 0);
    }
    
    public static bool IsTesting
    {
        get => PlayerPrefs.GetInt(Constant.IsTesting, 0) == 1;
        set
        {
            PlayerPrefs.SetInt(Constant.IsTesting, value ? 1 : 0);
            Observer.DebugChanged?.Invoke();
        }
    }

    public static int CurrentLevel
    {
        get => GetInt(Constant.IndexLevelCurrent, 1);

        set
        {
            SetInt(Constant.IndexLevelCurrent, value>=1?value:1);
            Observer.CurrentLevelChanged?.Invoke();
        }
    }

    public static int MoneyTotal
    {
        get => GetInt(Constant.MoneyTotal, 0);
        set
        {
            Observer.MoneyChanged?.Invoke(value - MoneyTotal);
            SetInt(Constant.MoneyTotal, Mathf.Max(0,value));
        }
    }

    public static int ProgressAmount
    {
        get => GetInt(Constant.ProgressAmount, 0);
        set => SetInt(Constant.ProgressAmount, value);
    }
    
    public static bool IsItemEquipped(string itemIdentity)
    {
        return GetBool($"{Constant.EquipItem}_{IdItemUnlocked}");
    }

    public static void SetItemEquipped(string itemIdentity, bool isEquipped = true)
    {
        SetBool($"{Constant.EquipItem}_{IdItemUnlocked}", isEquipped);
        Observer.EquipItem?.Invoke(itemIdentity);
    }

    public static string IdItemUnlocked = "";

    public static bool IsItemUnlocked
    {
        get => GetBool($"{Constant.UnlockItem}_{IdItemUnlocked}");
        set => SetBool($"{Constant.UnlockItem}_{IdItemUnlocked}", value);
    }
    #endregion
    
    #region SETTING_DATA

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
        set => SetBool(Constant.VibrateState, value);
    }

    #endregion
    
    #region DAILY_REWARD
    public static int CurrentDailyReward
    {
        get => GetInt(Constant.DailyRewardDayIndex, 1);
        set => SetInt(Constant.DailyRewardDayIndex, value);
    }

    public static string LastDailyRewardClaimed
    {
        get => GetString(Constant.LastDailyRewardClaim, DateTime.Now.AddDays(-1).ToString());
        set => SetString(Constant.LastDailyRewardClaim, value);
    }

    #endregion
    
}

public static partial class Data
{
    private static bool GetBool(string key, bool defaultValue = false) =>
        PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) > 0;

    private static void SetBool(string id, bool value) => PlayerPrefs.SetInt(id, value ? 1 : 0);

    private static int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
    private static void SetInt(string id, int value) => PlayerPrefs.SetInt(id, value);

    private static string GetString(string key, string defaultValue) => PlayerPrefs.GetString(key, defaultValue);
    private static void SetString(string id, string value) => PlayerPrefs.SetString(id, value);
}