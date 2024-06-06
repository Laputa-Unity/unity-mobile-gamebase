using System;
using UnityEngine;

[Serializable]
public partial class PlayerData
{
    [SerializeField] private bool isTesting;
    [SerializeField] private int currentLevelIndex;
    [SerializeField] private int currentMoney;

    public bool IsTesting
    {
        get => isTesting;
        set
        {
            isTesting = value;
            Observer.DebugChanged?.Invoke();
        }
    }

    public int CurrentLevelIndex
    {
        get => currentLevelIndex;
        set
        {
            currentLevelIndex = value;
            Observer.CurrentLevelChanged?.Invoke();
        }
    }

    public int CurrentMoney
    {
        get => currentMoney;
        set
        {
            Observer.MoneyChanged?.Invoke(value - currentMoney);
            currentMoney = value;
        }
    }
}
