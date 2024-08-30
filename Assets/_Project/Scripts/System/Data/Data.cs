using System.IO;
using UnityEngine;

public static class Data 
{
    public static PlayerData PlayerData;
    public static string SavePath = Application.persistentDataPath + "/player_data.json";
    public static void SaveData()
    {
        string jsonData = JsonUtility.ToJson(PlayerData, true);
        File.WriteAllText(SavePath, jsonData);
        Debug.Log("<color=green>Save player data succeed </color>");
    }

    public static void LoadData()
    {
        if (File.Exists(SavePath))
        {
            string jsonData = File.ReadAllText(SavePath);
            PlayerData = JsonUtility.FromJson<PlayerData>(jsonData);
            Debug.Log("<color=green>Load player data succeed </color>");
        }
        else
        {
            PlayerData = new PlayerData();
            Debug.Log("<color=green>Create new player data ... </color>");
        }
    }

    public static void ClearData()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("<color=green>Clear player data succeed </color>");
        }
        else
        {
            Debug.LogWarning("No save file found to delete!");
        }
    }
}
