using System;
public static partial class Data
{
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
}

public static partial class Constant
{
    public const string LastDailyRewardClaim = "LAST_DAILY_REWARD_CLAIM";
    public const string DailyRewardDayIndex = "DAILY_REWARD_DAY_INDEX";
}