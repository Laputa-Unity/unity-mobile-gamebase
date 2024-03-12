using UnityEngine;

public static partial class Data
{
    private static bool GetBool(string key, bool defaultValue = false) =>
        PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) > 0;

    private static void SetBool(string id, bool value) => PlayerPrefs.SetInt(id, value ? 1 : 0);

    private static int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
    private static void SetInt(string id, int value) => PlayerPrefs.SetInt(id, value);

    private static string GetString(string key, string defaultValue) => PlayerPrefs.GetString(key, defaultValue);
    private static void SetString(string id, string value) => PlayerPrefs.SetString(id, value);
}

public partial class Constant
{
    public const string LoadingScene = "LoadingScene";
    public const string GameplayScene = "GamePlayScene";
}