using System;
using UnityEngine;

public partial class PlayerData
{
    [SerializeField] private int currentDailyReward = 1;
    [SerializeField] private string lastDailyRewardClaimed = DateTime.Now.AddDays(-1).ToString();

    public int CurrentDailyReward
    {
        get => currentDailyReward;
        set => currentDailyReward = value;
    }

    public string LastDailyRewardClaimed
    {
        get => lastDailyRewardClaimed;
        set => lastDailyRewardClaimed = value;
    }
}