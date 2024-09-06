using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public static class Data 
{
    public static PlayerData PlayerData;
    public static string SavePath = Application.persistentDataPath + "/player_data.json";
    public static void SaveData()
    {
        var jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        string jsonData = JsonConvert.SerializeObject(PlayerData, Formatting.Indented, jsonSettings);

        // Encrypt the JSON data
        string encryptedData = EncryptionHelper.Encrypt(jsonData);
        File.WriteAllText(SavePath, encryptedData);
        Debug.Log("<color=green>Save player data (encrypted) succeed</color>");
    }

    public static void LoadData()
    {
        if (File.Exists(SavePath))
        {
            string encryptedData = File.ReadAllText(SavePath);

            // Decrypt the data before loading
            string decryptedData = EncryptionHelper.Decrypt(encryptedData);
            PlayerData = JsonConvert.DeserializeObject<PlayerData>(decryptedData);

            Debug.Log("<color=green>Load player data (decrypted) succeed</color>");
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

    public static async Task UpdateData(string jsonContent)
    {
        Debug.Log(Application.persistentDataPath);
        await File.WriteAllTextAsync(SavePath, jsonContent);
        string jsonData = await File.ReadAllTextAsync(SavePath);
        PlayerData = JsonUtility.FromJson<PlayerData>(jsonData);
        Debug.Log("<color=green>Update player data succeed </color>");
    }
}
