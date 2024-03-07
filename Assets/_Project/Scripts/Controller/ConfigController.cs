using UnityEngine;

public class ConfigController : SingletonDontDestroy<ConfigController>
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private SoundConfig soundConfig;
    [SerializeField] private DailyRewardConfig dailyRewardConfig;
    [SerializeField] private ItemConfig itemConfig;

    public static GameConfig Game;
    public static SoundConfig Sound;
    public static DailyRewardConfig DailyRewardConfig;
    public static ItemConfig ItemConfig;
    
    protected override void Awake()
    {
        base.Awake();
        Game = gameConfig;
        Sound = soundConfig;
        DailyRewardConfig = dailyRewardConfig;
        ItemConfig = itemConfig;
    }

    public void Initialize()
    {
        ItemConfig.Initialize();
    }
}