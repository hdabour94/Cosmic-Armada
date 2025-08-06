using UnityEngine;

public class EnemyWeapon : WeaponBase
{
    [Header("Enemy Settings")]
    [SerializeField] private bool targetPlayer = true;

     private bool isInitialized = false; // متغير أمان إضافي

    // --- الدالة الجديدة ---
    public void InitializeFromData(EnemyData_SO data)
    {
        if (data == null || data.projectilePrefab == null)
        {
            isInitialized = false;
            return;
        }

        // بما أننا داخل EnemyWeapon، يمكننا الوصول إلى "settings" المحمي
        settings = new WeaponSettings();

        // املأ الإعدادات بالبيانات
        settings.projectilePrefab = data.projectilePrefab;
        settings.firePoint = this.transform;
        settings.fireRate = data.fireRate;
        settings.damage = data.projectileDamage;
        settings.projectileSpeed = data.projectileSpeed;

        if (targetPlayer && LevelManager.Instance?.CurrentPlayerInstance != null)
        {
            SetTarget(LevelManager.Instance.CurrentPlayerInstance.transform);
        }
        
        isInitialized = true;
    }

    protected override bool CanFire()
    {
        return isInitialized;
    }
    

    protected override void SetupProjectile(GameObject projectile)
    {
        base.SetupProjectile(projectile);
        
        if (target != null && settings.firePoint != null) // Added null check for firePoint
        {
            Vector2 direction = (target.position - settings.firePoint.position).normalized;
            // Ensure Rigidbody2D is present and linearVelocity is used
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * settings.projectileSpeed;
            }
            else
            {
                Debug.LogWarning("Projectile missing Rigidbody2D component!", projectile);
            }
        }
    }
}