using System;

public partial class PlayerData
{
    public int currentDailyReward = 1;
    public string lastDailyRewardClaimed = DateTime.Now.AddDays(-1).ToString();
}