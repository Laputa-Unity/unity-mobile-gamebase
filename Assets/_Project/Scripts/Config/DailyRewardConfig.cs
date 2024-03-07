using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DailyRewardConfig", menuName = "ScriptableObject/DailyRewardConfig")]
public class DailyRewardConfig : ScriptableObject
{
    [SerializeField] private List<DailyRewardData> dailyRewardData;
    [SerializeField] private List<DailyRewardData> dailyRewardDataLoop;

    public List<DailyRewardData> DailyRewardData
    {
        get => dailyRewardData;
        set => dailyRewardData = value;
    }

    public List<DailyRewardData> DailyRewardDataLoop
    {
        get => dailyRewardDataLoop;
        set => dailyRewardDataLoop = value;
    }
}

[Serializable]
public class DailyRewardData
{
    [SerializeField] private DailyRewardType dailyRewardType;
    [SerializeField] private Sprite icon;
    [SerializeField] private string skinID;
    [SerializeField] private int value;

    public DailyRewardType DailyRewardType
    {
        get => dailyRewardType;
        set => dailyRewardType = value;
    }

    public Sprite Icon
    {
        get => icon;
        set => icon = value;
    }

    public string SkinID
    {
        get => skinID;
        set => skinID = value;
    }

    public int Value
    {
        get => value;
        set => this.value = value;
    }
}

public enum DailyRewardType
{
    Currency,
    Skin,
}