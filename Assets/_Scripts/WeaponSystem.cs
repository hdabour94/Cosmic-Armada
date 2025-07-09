using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private bool isAutoFire = true; // اجعلها قابلة للتعديل من الـ Inspector

    private float nextFireTime = 0f;

    void Update()
    {
        // إذا كان الإطلاق الآلي مفعلًا، أو إذا كان اللاعب يضغط على زر الإطلاق (للحاسوب)
        if (isAutoFire || Input.GetButton("Fire1"))
        {
            if (Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Fire()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("Projectile Prefab or Fire Point is not set!");
        }
    }
}