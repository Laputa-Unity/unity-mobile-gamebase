using System;
using UnityEngine;

[Serializable]
public partial class PlayerData
{
    [SerializeField] private bool isTesting;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentMoney = 0;

    public bool IsTesting
    {
        get => isTesting;
        set => isTesting = value;
    }

    public int CurrentLevel
    {
        get => currentLevel;
        set
        {
            currentLevel = value;
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
