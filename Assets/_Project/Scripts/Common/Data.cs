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

    public static bool IsClaimedTodayDailyReward()
    {
        return (int) (DateTime.Now - DateTime.Parse(LastDailyRewardClaimed)).TotalDays == 0;
    }
    
    public static bool IsStartLoopingDailyReward
    {
        get => PlayerPrefs.GetInt(Constant.IsStartLoopingDailyReward, 0) == 1;
        set => PlayerPrefs.SetInt(Constant.IsStartLoopingDailyReward, value ? 1 : 0);
    }

    public static string DateTimeStart
    {
        get => GetString(Constant.DateTimeStart, DateTime.Now.ToString());
        set => SetString(Constant.DateTimeStart, value);
    }

    public static int TotalPlayedDays =>
        (int) (DateTime.Now - DateTime.Parse(DateTimeStart)).TotalDays + 1;

    public static int DailyRewardDayIndex
    {
        get => GetInt(Constant.DailyRewardDayIndex, 1);
        set => SetInt(Constant.DailyRewardDayIndex, value);
    }

    public static string LastDailyRewardClaimed
    {
        get => GetString(Constant.LastDailyRewardClaim, DateTime.Now.AddDays(-1).ToString());
        set => SetString(Constant.LastDailyRewardClaim, value);
    }
    
    public static int TotalClaimDailyReward
    {
        get => GetInt(Constant.TotalClaimDailyReward, 0);
        set => SetInt(Constant.TotalClaimDailyReward, value);
    }

    #endregion

    #region PLAYFAB_DATA
    
    public static string PlayfabLoginId
    {
        get => GetString(Constant.PlayfabLoginID, null);
        set => SetString(Constant.PlayfabLoginID, value);
    }
    
    public static string PlayerName
    {
        get => GetString(Constant.PlayerName, null);
        set => SetString(Constant.PlayerName, value);
        
    }

    public static string PlayerId
    {
        get => GetString(Constant.PlayerID, null);
        set => SetString(Constant.PlayerID, value);
        
    }
    
    public static string PlayerCountryCode
    {
        get => GetString(Constant.PlayerCountryCode, null);
        set => SetString(Constant.PlayerCountryCode, value);
    }
    #endregion

    #region FIREBASE

    // TOGGLE LEVEL AB TESTING? 0:NO, 1:YES
    public static int DEFAULT_USE_LEVEL_AB_TESTING = 0;
    public static int UseLevelABTesting
    {
        get => PlayerPrefs.GetInt(Constant.UseLevelAbTesting, DEFAULT_USE_LEVEL_AB_TESTING);
        set => PlayerPrefs.SetInt(Constant.UseLevelAbTesting, value);
    }

    // SET LEVEL TO ENABLE INTERSTITIAL
    public static int DEFAULT_LEVEL_TURN_ON_INTERSTITIAL = 5;
    public static int LevelTurnOnInterstitial
    {
        get => PlayerPrefs.GetInt(Constant.LevelTurnONInterstitial,
            DEFAULT_LEVEL_TURN_ON_INTERSTITIAL);
        set => PlayerPrefs.SetInt(Constant.LevelTurnONInterstitial, value);
    }
    
    // SET COUNTER VARIABLE
    public static int DEFAULT_COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL = 2;
    public static int CounterNumbBetweenTwoInterstitial
    {
        get => PlayerPrefs.GetInt(Constant.CounterNumberBetweenTwoInterstitial, DEFAULT_COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL);
        set => PlayerPrefs.SetInt(Constant.CounterNumberBetweenTwoInterstitial, value);
    }
    
    // SET TIME TO ENABLE BETWEEN 2 INTERSTITIAL (ON WIN,LOSE,REPLAY GAME)
    public static int DEFAULT_SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL = 30;
    public static int TimeWinBetweenTwoInterstitial
    {
        get => PlayerPrefs.GetInt(Constant.SpaceTimeWinBetweenTwoInterstitial, DEFAULT_SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL);
        set => PlayerPrefs.SetInt(Constant.SpaceTimeWinBetweenTwoInterstitial, value);
    }
    
    // TOGGLE SHOW INTERSTITIAL ON LOSE GAME ? 0:NO, 1:YES
    public static int DEFAULT_SHOW_INTERSTITIAL_ON_LOSE_GAME = 0;
    public static int UseShowInterstitialOnLoseGame
    {
        get => PlayerPrefs.GetInt(Constant.ShowInterstitialONLoseGame, DEFAULT_SHOW_INTERSTITIAL_ON_LOSE_GAME);
        set => PlayerPrefs.SetInt(Constant.ShowInterstitialONLoseGame, value);
    }
    
    // SET TIME TO ENABLE BETWEEN 2 INTERSTITIAL (ON LOSE GAME)
    public static int DEFAULT_SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL = 45;
    public static int TimeLoseBetweenTwoInterstitial
    {
        get => PlayerPrefs.GetInt(Constant.SpaceTimeLoseBetweenTwoInterstitial, DEFAULT_SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL);
        set => PlayerPrefs.SetInt(Constant.SpaceTimeLoseBetweenTwoInterstitial, value);
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