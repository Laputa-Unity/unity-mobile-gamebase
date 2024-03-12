using UnityEngine;

public static partial class Data
{
    public static string CurrentPlayerSkin
    {
        get => PlayerPrefs.GetString(Constant.CurrentPlayerSkin, $"{ItemType.PlayerSkin}_0");
        set
        {
            PlayerPrefs.SetString(Constant.CurrentPlayerSkin, value);
            Observer.EquipPlayerSkin?.Invoke(value);
        }
    }
}

public static partial class Constant
{
    public const string CurrentPlayerSkin = "CURRENT_PLAYER_SKIN";
}