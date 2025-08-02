// START OF FILE WeaponBase.txt
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [System.Serializable]
    public class WeaponSettings
    {
        public GameObject projectilePrefab;
        public Transform firePoint;
        public float fireRate = 0.5f;
        public int damage = 10;
        public float projectileSpeed = 15f;
    }

    // CHANGED: Made 'settings' public
    public WeaponSettings settings; 
    protected float nextFireTime;
    protected Transform target;

    protected virtual void Start()
    {
        if (settings.firePoint == null)
        {
            settings.firePoint = transform;
        }
    }

    protected virtual void Update()
    {
        if (CanFire() && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + settings.fireRate;
        }
    }

    protected abstract bool CanFire();

    protected virtual void Fire()
    {
        if (settings.projectilePrefab == null || settings.firePoint == null) return;

        GameObject projectile = Instantiate(
            settings.projectilePrefab,
            settings.firePoint.position,
            settings.firePoint.rotation
        );

        SetupProjectile(projectile);
    }

    protected virtual void SetupProjectile(GameObject projectile)
    {
        Projectile projComponent = projectile.GetComponent<Projectile>();
        if (projComponent != null)
        {
            projComponent.Initialize(settings.damage, settings.projectileSpeed);
        }
    }

    public void SetTarget(Transform newTarget) => target = newTarget;
}