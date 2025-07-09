using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static readonly string SAVE_FILE_NAME = "playerdata.json";

    public static void SavePlayer(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        File.WriteAllText(path, json);
        Debug.Log("Game Saved to: " + path);
    }

    public static PlayerData LoadPlayer()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Game Loaded from: " + path);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found. Creating a new one.");
            // إرجاع بيانات افتراضية إذا لم يتم العثور على ملف حفظ
            return new PlayerData { playerLevel = 1, playerXP = 0, xpToNextLevel = 100, coins = 0 };
        }
    }
}