using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [System.Serializable]
    public class WeaponSettings
    {
        public GameObject projectilePrefab;
        public Transform firePoint;
        [Tooltip("The maximum delay between shots. The actual delay will be random.")]
        public float fireRate = 0.5f;
        public int damage = 10;
        public float projectileSpeed = 15f;
    }
    [Tooltip("The core settings for this weapon. Can be configured in the Inspector.")]
    [SerializeField]
    protected WeaponSettings settings;

    //public WeaponSettings settings;
    protected float nextFireTime;
    protected Transform target;

    protected virtual void Start()
    {
        if (settings == null) settings = new WeaponSettings();

        if (settings.firePoint == null)
        {
            settings.firePoint = transform;
        }
        // لا نحسب وقت الإطلاق هنا، بل في Update/Fire لأول مرة
    }

    protected virtual void Update()
    {
        if (CanFire() && Time.time >= nextFireTime)
        {
            Fire();
            // بعد إطلاق النار، احسب وقت الإطلاق التالي
            CalculateNextFireTime();
        }
    }

    protected virtual void CalculateNextFireTime()
    {
        // السلوك الافتراضي: فاصل زمني ثابت
        nextFireTime = Time.time + settings.fireRate;
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