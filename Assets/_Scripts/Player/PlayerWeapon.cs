// PlayerWeapon.cs

using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerWeapon : WeaponBase
{
    [Header("Player Settings")]
    [SerializeField] private bool autoFire = true;
    [SerializeField] private KeyCode manualFireKey = KeyCode.Space;

    [Header("Weapon Assets")]
    [SerializeField] private GameObject projectileToUse; // <<<--- حقل جديد

    private bool isInitialized = false; // متغير لمنع إطلاق النار قبل التهيئة

    public void Initialize(CharacterStats_SO stats)
    {
        settings = new WeaponSettings
        {
            projectilePrefab = this.projectileToUse, // <<<--- استخدم الحقل الجديد
            firePoint = this.transform,
            fireRate = stats.fireRate,
            damage = stats.strength,
            projectileSpeed = 15f 
        };
        isInitialized = true;
    }

    protected override bool CanFire()
    {
        // لا تطلق النار إلا إذا تم تهيئة السلاح
      //  if (!isInitialized) return false;

        return autoFire || Input.GetKey(manualFireKey);
    }

    protected override void Fire()
    {
        base.Fire();
    }
}