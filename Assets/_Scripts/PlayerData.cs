using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int version;
    public int playerLevel;
    public int playerXP;
    public int xpToNextLevel;
    public int coins;
    
    // --- هذا هو التعريف الوحيد الذي نحتاجه ---
    public string currentShipID;
    public List<string> unlockedShipIDs;
    // ------------------------------------------
    
    // يمكن حذف upgradeLevels مؤقتًا إذا لم تكن تستخدمه
    // public int[] upgradeLevels; 
}