using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpinConfig", menuName = "ScriptableObject/SpinConfig")]
public class SpinConfig : ScriptableObject
{
    public List<SlotItemData> slotData;
}

[Serializable]
public class SlotItemData
{
    public SlotItemDataType type;
    public int minWeight;
    public int maxWeight;
    public int amount;
    public Sprite sprite;
}

public enum SlotItemDataType
{
    Money,
}
