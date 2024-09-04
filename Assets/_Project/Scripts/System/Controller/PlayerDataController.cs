using CustomInspector;
using UnityEngine;

public class PlayerDataController : SingletonDontDestroy<PlayerDataController>
{
    [ReadOnly] [SerializeField] private PlayerData playerDataReader;

    private static bool _isFetchPlayerDataSucceed;
    private bool _cacheFirstPlaying;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        Initialize();
    }

    private static void Initialize()
    {
        Data.LoadData();
        _isFetchPlayerDataSucceed = true;
    }

    protected override void Awake()
    {
        base.Awake();
        playerDataReader = Data.PlayerData;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!_isFetchPlayerDataSucceed) return;
        if (!hasFocus && Data.PlayerData.IsFirstPlaying)
        {
            Data.PlayerData.IsFirstPlaying = false;
            _cacheFirstPlaying = true;
        }

        if (hasFocus && _cacheFirstPlaying)
        {
            Data.PlayerData.IsFirstPlaying = true;
        }

        Data.SaveData();
    }

    // Not working on simulator
    void OnApplicationPause(bool pauseStatus)
    {
        SaveData();
    }

    // Not working on mobile device
    private void OnApplicationQuit()
    {
        if (!_isFetchPlayerDataSucceed) return;
        Data.PlayerData.IsFirstPlaying = false;

        Data.SaveData();
    }

    public void SaveData()
    {
        if (!_isFetchPlayerDataSucceed) return;
        Data.SaveData();
    }

    public void LoadData()
    {
        Data.LoadData();
    }
}
