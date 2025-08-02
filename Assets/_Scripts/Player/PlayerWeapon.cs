// PlayerWeapon.cs

using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerWeapon : WeaponBase
{
    [Header("Player Settings")]
    [SerializeField] private bool autoFire = true;
    [SerializeField] private KeyCode manualFireKey = KeyCode.Space;

    private bool isInitialized = false; // متغير لمنع إطلاق النار قبل التهيئة

    // أزل دالة Start() من هنا تمامًا لتجنب التهيئة المبكرة

    public void Initialize(CharacterStats_SO stats)
    {
        // نقوم بإنشاء إعدادات السلاح بناءً على الإحصائيات التي تم تمريرها
        settings = new WeaponSettings
        {
            // نفترض أن projectilePrefab يتم إسناده في الـ Inspector الخاص بـ PlayerWeapon
            projectilePrefab = this.settings.projectilePrefab,
            firePoint = this.transform, // أو firePoint مخصص
            fireRate = stats.fireRate,
            damage = stats.strength, // مثال: الضرر يساوي القوة
            projectileSpeed = 15f // قيمة افتراضية أو يمكن أخذها من الإحصائيات أيضًا
        };
        
        isInitialized = true;
    }

    protected override bool CanFire()
    {
        // لا تطلق النار إلا إذا تم تهيئة السلاح
        if (!isInitialized) return false;

        return autoFire || Input.GetKey(manualFireKey);
    }

    protected override void Fire()
    {
        base.Fire();
    }
}