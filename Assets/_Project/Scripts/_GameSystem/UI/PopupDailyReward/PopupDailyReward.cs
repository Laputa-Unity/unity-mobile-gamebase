using UnityEngine;
using System;
using System.Collections.Generic;
using CustomInspector;

public class PopupDailyReward : Popup
{
    [SerializeField] private GameObject btnWatchVideo;
    [SerializeField] private GameObject btnClaim;
    [SerializeField] private List<DailyRewardItem> dailyRewardItems;

    [ReadOnly] public DailyRewardItem currentItem;

    protected override void BeforeShow()
    {
        base.BeforeShow();
        Setup();
    }

    protected override void AfterHidden()
    {
        base.AfterHidden();
        if (!PopupController.Instance.Get<PopupHome>().isActiveAndEnabled)
        {
            GameManager.Instance.gameState = GameState.PlayingGame;
            PopupController.Instance.Hide<PopupUI>();
        }
    }

    private bool IsCurrentItem(int index)
    {
        return Data.CurrentDailyReward == index;
    }

    public void Setup()
    {
        PopupController.Instance.Show<PopupUI>();
        SetUpItems();
    }

    private void SetUpItems()
    {
        int week = (Data.CurrentDailyReward - 1) / 7;
        if (IsClaimTodayDailyReward()) week = (Data.CurrentDailyReward - 2) / 7;
        
        for (var i = 0; i < 7; i++)
        {
            int day = i + 7 * week;
            DailyRewardData data = DailyRewardController.Instance.GetDailyRewardData(day);
            var item = dailyRewardItems[i];
            item.SetUp(day, data, this);
            if (IsCurrentItem(item.dayIndex)) currentItem = item;
        }

        if (currentItem)
        {
            if (currentItem.DailyRewardItemState == DailyRewardItemState.ReadyToClaim)
            {
                btnWatchVideo.SetActive(currentItem.DailyRewardData.DailyRewardType == DailyRewardType.Money);
                btnClaim.SetActive(true);
            }
            else
            {
                btnWatchVideo.SetActive(false);
                btnClaim.SetActive(false);
            }
        }
        else
        {
            btnWatchVideo.SetActive(false);
            btnClaim.SetActive(false);
        }
           
    }

    public void OnClickBtnClaimX5Video()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        OnClaimReward(true);
    }

    public void OnClickBtnClaim()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        OnClaimReward(false);
    }

    private void OnClaimReward(bool isX5Reward)
    {
        Data.LastDailyRewardClaimed = DateTime.Now.ToString();
        Data.CurrentDailyReward++;

        currentItem.OnClaim(isX5Reward);
        
        Setup();
    }

    public void OnClickNextDay()
    {
        Data.LastDailyRewardClaimed = DateTime.Now.AddDays(-1).ToString();
        Setup();
    }

    public bool IsClaimTodayDailyReward()
    {
        if (DateTime.TryParse(Data.LastDailyRewardClaimed, out var dateTimeValue))
        {
            return DateTime.Now.Date == dateTimeValue.Date;
        }

        return false;
    }
}