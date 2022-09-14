using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

public static partial class Data
{
    #region GAME_DATA
    
    public static bool IsFirstOpenGame
    {
        get => PlayerPrefs.GetInt(Constant.IS_FIRST_OPEN_GAME, 0) == 1;
        set => PlayerPrefs.SetInt(Constant.IS_FIRST_OPEN_GAME, value ? 1 : 0);
    }
    
    public static bool IsTesting
    {
        get => PlayerPrefs.GetInt(Constant.IS_TESTING, 0) == 1;
        set => PlayerPrefs.SetInt(Constant.IS_TESTING, value ? 1 : 0);
    }
    
    public static int CurrentLevel
    {
        get { return GetInt(Constant.INDEX_LEVEL_CURRENT, 1); }

        set
        {
            SetInt(Constant.INDEX_LEVEL_CURRENT, value>=1?value:1);
            EventController.CurrentLevelChanged?.Invoke();
        }
    }

    public static int GetDailyRewardClaimed(DateTime dateTime)
    {
        return PlayerPrefs.GetInt($"Claimed_DailyReward_{dateTime.Day}/{dateTime.Month}/{dateTime.Year}", 0);
    }

    public static void SetDailyClaimed(bool isOwned = true)
    {
        PlayerPrefs.SetInt($"Claimed_DailyReward_{DateTime.Today.Day}/{DateTime.Today.Month}/{DateTime.Today.Year}", isOwned ? 1 : 0);
    }
    
    #endregion
    
    #region SOUND_DATA

    public static bool SoundState
    {
        get => GetBool(Constant.SOUND_STATE, true);
        set => SetBool(Constant.SOUND_STATE, value);
    }

    public static bool MusicState
    {
        get => GetBool(Constant.MUSIC_STATE, true);
        set => SetBool(Constant.MUSIC_STATE, value);
    }

    public static bool VibrateState
    {
        get => GetBool(Constant.VIBRATE_STATE, false);
        set => SetBool(Constant.VIBRATE_STATE, value);
    }

    #endregion
    
    #region DAILY_REWARD

    public static bool IsClaimedTodayDailyReward()
    {
        return (int) (DateTime.Now - DateTime.Parse(LastDailyRewardClaimed)).TotalDays == 0;
    }
    
    public static bool IsStartLoopingDailyReward
    {
        get => PlayerPrefs.GetInt(Constant.IS_START_LOOPING_DAILY_REWARD, 0) == 1;
        set => PlayerPrefs.SetInt(Constant.IS_START_LOOPING_DAILY_REWARD, value ? 1 : 0);
    }

    public static string DateTimeStart
    {
        get => GetString(Constant.DATE_TIME_START, DateTime.Now.ToString());
        set => SetString(Constant.DATE_TIME_START, value);
    }

    public static int TotalPlayedDays =>
        (int) (DateTime.Now - DateTime.Parse(DateTimeStart)).TotalDays + 1;

    public static int DailyRewardDayIndex
    {
        get => GetInt(Constant.DAILY_REWARD_DAY_INDEX, 1);
        set => SetInt(Constant.DAILY_REWARD_DAY_INDEX, value);
    }

    public static string LastDailyRewardClaimed
    {
        get => GetString(Constant.LAST_DAILY_REWARD_CLAIM, DateTime.Now.AddDays(-1).ToString());
        set => SetString(Constant.LAST_DAILY_REWARD_CLAIM, value);
    }
    
    public static int TotalClaimDailyReward
    {
        get => GetInt(Constant.TOTAL_CLAIM_DAILY_REWARD, 0);
        set => SetInt(Constant.TOTAL_CLAIM_DAILY_REWARD, value);
    }

    #endregion

    #region PLAYER_DATA
    
    public static int CurrencyTotal
    {
        get => GetInt(Constant.CURRENCY_TOTAL, 0);
        set
        {
            EventController.SaveCurrencyTotal?.Invoke();
            SetInt(Constant.CURRENCY_TOTAL, value);
            EventController.CurrencyTotalChanged?.Invoke();
        }
    }

    public static int ProgressAmount
    {
        get => GetInt(Constant.PROGRESS_AMOUNT, 0);
        set => SetInt(Constant.PROGRESS_AMOUNT, value);
    }

    public static int CurrentEquippedSkin
    {
        get => GetInt(Constant.CURRENT_EQUIPED_SKIN, 0);

        set
        {
            SetInt(Constant.CURRENT_EQUIPED_SKIN, value);
        }
    }

    #endregion

    #region SKIN_DATA

    public static string IdSkinCheckUnlocked = "";

    public static bool IsSkinUnlocked
    {
        get => GetBool(IdSkinCheckUnlocked, false);
        set => SetBool(IdSkinCheckUnlocked, value);
    }

    #endregion
    
    #region IAP_DATA

    public static bool IsIAPPackUnlocked(string name)
    {
        return GetBool($"IAP_{name}");
    }

    public static void SetIAPPack(string name, bool isOwned = true)
    {
        SetBool($"IAP_{name}", isOwned);
    }

    #endregion

    #region IAP_DATA

    public static string IdIAPCheckUnlocked = "";

    public static bool IsIAPUnlocked
    {
        get => GetBool(IdIAPCheckUnlocked, false);
        set => SetBool(IdIAPCheckUnlocked, value);
    }

    #endregion

    #region PLAYFAB_DATA
    
    public static string PlayfabLoginId
    {
        get => GetString(Constant.PLAYFAB_LOGIN_ID, null);
        set => SetString(Constant.PLAYFAB_LOGIN_ID, value);
    }
    
    public static string PlayerName
    {
        get => GetString(Constant.PLAYER_NAME, null);
        set => SetString(Constant.PLAYER_NAME, value);
        
    }

    public static string PlayerId
    {
        get => GetString(Constant.PLAYER_ID, null);
        set => SetString(Constant.PLAYER_ID, value);
        
    }
    
    public static string PlayerCountryCode
    {
        get => GetString(Constant.PLAYER_COUNTRY_CODE, null);
        set => SetString(Constant.PLAYER_COUNTRY_CODE, value);
    }
    
    public static PlayerProfileModel PlayerProfile;
    #endregion

    #region FIREBASE

    // TOGGLE LEVEL AB TESTING? 0:NO, 1:YES
    public static int DEFAULT_USE_LEVEL_AB_TESTING = 0;
    public static int UseLevelABTesting
    {
        get => PlayerPrefs.GetInt(Constant.USE_LEVEL_AB_TESTING, DEFAULT_USE_LEVEL_AB_TESTING);
        set => PlayerPrefs.SetInt(Constant.USE_LEVEL_AB_TESTING, value);
    }

    // SET LEVEL TO ENABLE INTERSTITIAL
    public static int DEFAULT_LEVEL_TURN_ON_INTERSTITIAL = 5;
    public static int LevelTurnOnInterstitial
    {
        get => PlayerPrefs.GetInt(Constant.LEVEL_TURN_ON_INTERSTITIAL,
            DEFAULT_LEVEL_TURN_ON_INTERSTITIAL);
        set => PlayerPrefs.SetInt(Constant.LEVEL_TURN_ON_INTERSTITIAL, value);
    }
    
    // SET COUNTER VARIABLE
    public static int DEFAULT_COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL = 2;
    public static int CounterNumbBetweenTwoInterstitial
    {
        get => PlayerPrefs.GetInt(Constant.COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL, DEFAULT_COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL);
        set => PlayerPrefs.SetInt(Constant.COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL, value);
    }
    
    // SET TIME TO ENABLE BETWEEN 2 INTERSTITIAL (ON WIN,LOSE,REPLAY GAME)
    public static int DEFAULT_SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL = 30;
    public static int TimeWinBetweenTwoInterstitial
    {
        get => PlayerPrefs.GetInt(Constant.SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL, DEFAULT_SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL);
        set => PlayerPrefs.SetInt(Constant.SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL, value);
    }
    
    // TOGGLE SHOW INTERSTITIAL ON LOSE GAME ? 0:NO, 1:YES
    public static int DEFAULT_SHOW_INTERSTITIAL_ON_LOSE_GAME = 0;
    public static int UseShowInterstitialOnLoseGame
    {
        get => PlayerPrefs.GetInt(Constant.SHOW_INTERSTITIAL_ON_LOSE_GAME, DEFAULT_SHOW_INTERSTITIAL_ON_LOSE_GAME);
        set => PlayerPrefs.SetInt(Constant.SHOW_INTERSTITIAL_ON_LOSE_GAME, value);
    }
    
    // SET TIME TO ENABLE BETWEEN 2 INTERSTITIAL (ON LOSE GAME)
    public static int DEFAULT_SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL = 45;
    public static int TimeLoseBetweenTwoInterstitial
    {
        get => PlayerPrefs.GetInt(Constant.SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL, DEFAULT_SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL);
        set => PlayerPrefs.SetInt(Constant.SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL, value);
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