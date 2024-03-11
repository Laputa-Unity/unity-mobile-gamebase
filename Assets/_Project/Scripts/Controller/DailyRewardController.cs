using UnityEngine;

public class DailyRewardController : SingletonDontDestroy<DailyRewardController>
{
    [SerializeField] private DailyRewardConfig dailyRewardConfig;

    public DailyRewardData GetDailyRewardData(int index)
    {
        return dailyRewardConfig.GetDailyRewardData(index);
    }
}
