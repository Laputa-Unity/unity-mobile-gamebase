using System;
using Pancake.Iap;
using Pancake.Monetization;
using UnityEngine;

public class AdsManager
{
    public static int TotalLevelWinLose;
    public static float TotalTimesPlay;
    public static bool IsInitialized => Advertising.IsInitialized;

    private static Action _callbackInterstitialCompleted;
    private static Action _callbackRewardCompleted;

    public static void Initialize()
    {
        Advertising.InterstitialAdCompletedEvent += OnInterstitialAdCompleted;
        Advertising.RewardedAdCompletedEvent += OnRewardAdCompleted;
    }

    private static void OnInterstitialAdCompleted(EInterstitialAdNetwork val)
    {
        _callbackInterstitialCompleted?.Invoke();
        _callbackInterstitialCompleted = null;
    }
    
    private static void OnRewardAdCompleted(ERewardedAdNetwork val)
    {
        _callbackRewardCompleted?.Invoke();
        _callbackRewardCompleted = null;
    }


    public static bool IsSufficientConditionToShowInter()
    {
        if (Data.CurrentLevel>Data.LevelTurnOnInterstitial && TotalLevelWinLose >= Data.CounterNumbBetweenTwoInterstitial && TotalTimesPlay>=Data.TimeWinBetweenTwoInterstitial)
        {
            return true;
        }

        return false;
    }

    public static void ShowInterstitial(Action callBack)
    {
        if (IsSufficientConditionToShowInter())
        {
            FirebaseManager.OnRequestInterstitial();
            if (Advertising.IsInterstitialAdReady())
            {
                FirebaseManager.OnShowInterstitial();
                
                _callbackInterstitialCompleted = callBack;
                Advertising.ShowInterstitialAd();
                Reset();
            }
            else
            {
                callBack?.Invoke();
            }
        }
        else
        {
            callBack?.Invoke();
        }
    }
    
    public static void ShowRewardAds(Action callBack)
    {
        FirebaseManager.OnRequestReward();
        if (Advertising.IsRewardedAdReady())
        {
            FirebaseManager.OnShowReward();
            
            _callbackRewardCompleted = callBack;
            Advertising.ShowRewardedAd();
        }
    }

    private static void Reset()
    {
        TotalTimesPlay = 0;
        TotalLevelWinLose = 0;
    }

    public static void ShowBanner()
    {
        FirebaseManager.OnShowBanner();
        
        Advertising.ShowBannerAd();
    }
    
    public static void HideBanner()
    {
        Advertising.HideBannerAd();
    }
}
