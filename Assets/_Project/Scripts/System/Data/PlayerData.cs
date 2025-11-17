using System;
using System.Globalization;
using UnityEngine;

[Serializable]
public partial class PlayerData
{
    [SerializeField] private bool isFirstPlaying = true;
    [SerializeField] private int currentLevelIndex = _minLevel;
    [SerializeField] private int currentEnergy;
    [SerializeField] private int currentGold;
    [SerializeField] private int currentDiamond;
    [SerializeField] private GameReward savingReward;
    [SerializeField] private string refillEnergyPoint = DateTime.UtcNow.ToString(Utility.DateTimeFormat, CultureInfo.InvariantCulture);
    
    private const int _minLevel = 1;
    
    public bool IsFirstPlaying
    {
        get => isFirstPlaying;
        set => isFirstPlaying = value;
    }

    public int CurrentLevelIndex
    {
        get => currentLevelIndex;
        set => currentLevelIndex = Mathf.Max(_minLevel,value);
    }

    public int CurrentEnergy
    {
        get => currentEnergy;
        set
        {
            Observer.EnergyChanged?.Invoke(value - currentEnergy);
            currentEnergy = Mathf.Clamp(value, 0, value);
            Observer.EnergyChangedDone?.Invoke();
        }
    }

    public int CurrentGold
    {
        get => currentGold;
        set
        {
            Observer.GoldChanged?.Invoke(value - currentGold);
            currentGold = value;
            Observer.GoldChangedDone?.Invoke();
        }
    }
    
    public int CurrentDiamond
    {
        get => currentDiamond;
        set
        {
            Observer.DiamondChanged?.Invoke(value - currentDiamond);
            currentDiamond = value;
            Observer.DiamondChangedDone?.Invoke();
        }
    }

    public GameReward SavingReward
    {
        get => savingReward;
        set => savingReward = value;
    }

    public string RefillEnergyPoint
    {
        get => refillEnergyPoint;
        set => refillEnergyPoint = value;
    }
}

[Serializable]
public class GameReward
{
    public int energyValue;
    public int goldValue;
    public int diamondValue;
}