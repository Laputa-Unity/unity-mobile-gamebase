using UnityEngine;

public static partial class Data
{
    public static bool IsTesting
    {
        get => PlayerPrefs.GetInt(Constant.IsTesting, 0) == 1;
        set
        {
            PlayerPrefs.SetInt(Constant.IsTesting, value ? 1 : 0);
            Observer.DebugChanged?.Invoke();
        }
    }

    public static int CurrentLevel
    {
        get => GetInt(Constant.CurrentLevel, 1);

        set
        {
            SetInt(Constant.CurrentLevel, value>=1?value:1);
            Observer.CurrentLevelChanged?.Invoke();
        }
    }

    public static int CurrentMoney
    {
        get => GetInt(Constant.CurrentMoney, 0);
        set
        {
            Observer.MoneyChanged?.Invoke(value - CurrentMoney);
            SetInt(Constant.CurrentMoney, Mathf.Max(0,value));
        }
    }

    public static string IdItemUnlocked = "";

    public static bool IsItemUnlocked
    {
        get => GetBool($"{Constant.UnlockItem}_{IdItemUnlocked}");
        set => SetBool($"{Constant.UnlockItem}_{IdItemUnlocked}", value);
    }
}

public static partial class Constant
{
    public const string IsTesting = "IS_TESTING";
    public const string DateTimeStart = "DATE_TIME_START";
    
    public const string CurrentMoney = "CURRENT_MONEY";
    public const string CurrentLevel = "CURRENT_LEVEL";
    public const string UnlockItem = "UNLOCK_ITEM";
}