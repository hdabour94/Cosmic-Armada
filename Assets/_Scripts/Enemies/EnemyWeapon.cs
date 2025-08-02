using UnityEngine;

public class EnemyWeapon : WeaponBase
{
    [Header("Enemy Settings")]
    [SerializeField] private bool targetPlayer = true;

    protected override void Start()
    {
        base.Start(); // Calls the base Start which initializes settings if null
        if (targetPlayer)
        {
            SetTarget(LevelManager.Instance?.CurrentPlayerInstance?.transform);
        }
    }

    protected override bool CanFire() => true;

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