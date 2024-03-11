using System;
using System.Collections.Generic;
using CustomInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "DailyRewardConfig", menuName = "ScriptableObject/DailyRewardConfig")]
public class DailyRewardConfig : ScriptableObject
{
    public List<DailyRewardData> dailyRewardData;
    public List<DailyRewardData> loopDailyRewardData;

    public DailyRewardData GetDailyRewardData(int index)
    {
        if (index < dailyRewardData.Count)
        {
            return dailyRewardData[index];
        }
        var newIndex = (index - dailyRewardData.Count) % loopDailyRewardData.Count;
        return loopDailyRewardData[newIndex];
    }
}

[Serializable]
public class DailyRewardData
{
    public DailyRewardType dailyRewardType;
    public Sprite icon;
    [ShowIf("dailyRewardType", DailyRewardType.Money)] public int value;
    [ShowIf("dailyRewardType", DailyRewardType.Skin)] public string skinID;

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
    Money,
    Skin,
}