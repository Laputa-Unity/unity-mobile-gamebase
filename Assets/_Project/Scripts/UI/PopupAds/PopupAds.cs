using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupAds : Popup
{
    protected override void BeforeShow()
    {
        base.BeforeShow();
        AdsManager.ShowBanner();
    }
    
    protected override void BeforeHide()
    {
        base.BeforeHide();
        AdsManager.HideBanner();
    }

    public void OnClickBtnShowInterstitialAds()
    {
        AdsManager.ShowInterstitial(()=>Debug.Log("Show inter"));
    }
    
    public void OnClickBtnShowRewardAds()
    {
        AdsManager.ShowRewardAds(()=>Data.CurrencyTotal+=500);
    }
}
