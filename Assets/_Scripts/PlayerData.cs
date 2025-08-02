// START OF FILE PlayerData.txt
﻿[System.Serializable]
public class PlayerData
{
    public int playerLevel;
    public int playerXP;
    public int xpToNextLevel; // هذا الحقل كان ناقصًا
    public int coins;
    public int version; // Added this field
    
    // يمكن إضافة حقول إضافية هنا
    public string[] unlockedShips;
    public int[] upgradeLevels;
}