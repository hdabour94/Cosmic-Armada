using UnityEngine;

[CreateAssetMenu(fileName = "New Ship Data", menuName = "GameData/Ship Data")]
public class ShipData_SO : ScriptableObject
{
    public string shipName;
    public Sprite icon;
    public GameObject prefab;
    public CharacterStats_SO stats; // احتفظ بهذا الاسم لتوافقية مع الأكواد الأخرى
    
    
    [Header("Upgrade System")]
    public WeaponType defaultWeapon = WeaponType.Standard;
    public UpgradePath[] upgradePaths;
    
    [System.Serializable]
    public class UpgradePath
    {
        public string pathName;
        public UpgradeStep[] steps;
    }

    [System.Serializable]
    public class UpgradeStep
    {
        public int cost;
        public StatModifier[] modifiers;
    }

    [System.Serializable]
    public class StatModifier
    {
        public StatType statType;
        public float value;
    }

    // دالة مساعدة لتهيئة الإحصائيات
    public void InitializeStats(CharacterStats_SO targetStats)
    {
      /*  targetStats.maxHP = this.maxHP;
        targetStats.strength = this.strength;
        targetStats.speed = this.speed;
        targetStats.fireRate = this.fireRate;*/
    }
}

public enum StatType { Health, Damage, Speed, FireRate }
public enum WeaponType { Standard, Spread, Laser }