// START OF FILE SaveSystem.txt
﻿using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = "PlayerSaves";
    private static readonly string SAVE_FILE = "save01.dat";

    public static void SavePlayer(PlayerData data)
    {
        string savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
        
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        
        string fullPath = Path.Combine(savePath, SAVE_FILE);
        string encryptedData = DataEncryptor.Encrypt(JsonUtility.ToJson(data));
        
        File.WriteAllText(fullPath, encryptedData);
        CreateBackup(fullPath);
    }

    public static PlayerData LoadPlayer()
    {
        string fullPath = Path.Combine(
            Application.persistentDataPath, 
            SAVE_FOLDER, 
            SAVE_FILE
        );

        if (File.Exists(fullPath))
        {
            string encryptedData = File.ReadAllText(fullPath);
            return JsonUtility.FromJson<PlayerData>(DataEncryptor.Decrypt(encryptedData));
        }
        
        return new PlayerData() { version = 1 };
    }

    private static void CreateBackup(string originalPath)
    {
        string backupPath = originalPath + ".bak";
        File.Copy(originalPath, backupPath, true);
    }
}

public static class DataEncryptor
{
    private static readonly string KEY = "YourEncryptionKey123";
    
    public static string Encrypt(string data)
    {
        // تنفيذ التشفير هنا (يمكن استخدام AES)
        return data; // مؤقت
    }
    
    public static string Decrypt(string data)
    {
        // تنفيذ فك التشفير هنا
        return data; // مؤقت
    }
}