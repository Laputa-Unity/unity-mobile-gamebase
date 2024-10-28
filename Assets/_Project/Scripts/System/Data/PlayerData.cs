using System;
using UnityEngine;

[Serializable]
public partial class PlayerData
{
    [SerializeField] private bool isFirstPlaying = true;
    [SerializeField] private int currentLevelIndex = 1;
    [SerializeField] private int currentMoney;

    public bool IsFirstPlaying
    {
        get => isFirstPlaying;
        set => isFirstPlaying = value;
    }

    public int CurrentLevelIndex
    {
        get => currentLevelIndex;
        set
        {
            currentLevelIndex = Mathf.Max(1, value);
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
