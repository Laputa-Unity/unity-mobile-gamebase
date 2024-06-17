using System;
using Custom.DataStorage;
using CustomInspector;
using UnityEngine;

public class PlayerDataController : SingletonDontDestroy<PlayerDataController>
{
    [ReadOnly] [SerializeField] private PlayerData playerDataReader;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        GameData.Init();

        GameData.TryGet("PlayerData", out PlayerData data);
        Data.PlayerData = data ?? new PlayerData();
    }

    protected override void Awake()
    {
        base.Awake();
        playerDataReader = Data.PlayerData;
    }

    private void OnApplicationQuit()
    {
        GameData.Set("PlayerData", Data.PlayerData);
        GameData.Save();
    }

    public void SaveData()
    {
        GameData.Save();
    }

    public void LoadData()
    {
        GameData.TryGet("PlayerData", out PlayerData data);
        Data.PlayerData = data ?? new PlayerData();
    }
}

public static class Data
{
    public static PlayerData PlayerData;
}
