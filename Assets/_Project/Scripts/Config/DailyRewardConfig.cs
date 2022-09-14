using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DailyRewardConfig", menuName = "ScriptableObject/DailyRewardConfig")]
public class DailyRewardConfig : ScriptableObject
{
    public List<DailyRewardData> DailyRewardDatas;
    public List<DailyRewardData> DailyRewardDatasLoop;
    
    
}

[Serializable]
public class DailyRewardData
{
    public DailyRewardType DailyRewardType;
    public Sprite Icon;
    public string SkinID;
    public int Value;
}

public enum DailyRewardType
{
    Currency,
    Skin,
}