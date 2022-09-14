using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupHome : Popup
{

    protected override void BeforeShow()
    {
        base.BeforeShow();
        SoundController.Instance.PlayBackground(SoundType.BackgroundHome);
        PopupController.Instance.Show<PopupUI>();
    }
    
    protected override void BeforeHide()
    {
        base.BeforeHide();
        PopupController.Instance.Hide<PopupUI>();
    }

    public void OnClickStart()
    {
        GameManager.Instance.StartGame();
    }

    public void OnClickDebug()
    {
        PopupController.Instance.Show<PopupDebug>();
    }

    public void OnClickSetting()
    {
        FirebaseManager.OnClickButtonSetting();
        PopupController.Instance.Show<PopupSetting>();
    }

    public void OnClickDailyReward()
    {
        FirebaseManager.OnClickButtonDailyReward();
        PopupController.Instance.Show<PopupDailyReward>();
    }

    public void OnClickShop()
    {
        FirebaseManager.OnClickButtonShop();
        PopupController.Instance.Show<PopupShop>();
    }
    
    public void OnClickTestAds()
    {
        PopupController.Instance.Show<PopupAds>();
    }
}
