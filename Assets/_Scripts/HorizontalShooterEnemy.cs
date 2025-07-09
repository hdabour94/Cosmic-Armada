using System.Collections;
using UnityEngine;

public class HorizontalShooterEnemy : BaseEnemy
{
    [Header("Horizontal Shooter Settings")]
    [SerializeField] private float horizontalRange = 5f; // مدى الحركة الأفقية
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private GameObject enemyProjectilePrefab;
    [SerializeField] private Transform firePoint;

    private Vector3 startPosition;
    private float timeSinceLastFire = 0f;
    private int direction = 1; // 1 لليمين، -1 لليسار

    protected override void Start()
    {
        base.Start(); // استدعاء دالة Start من السكربت الأب
        startPosition = transform.position;
        timeSinceLastFire = Random.Range(0, fireRate); // يبدأ كل عدو بإطلاق نار في وقت مختلف
    }

    protected override void Move()
    {
        // الحركة الأفقية
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // تغيير الاتجاه عند الوصول إلى حدود النطاق
        if (transform.position.x > startPosition.x + horizontalRange)
        {
            direction = -1;
        }
        else if (transform.position.x < startPosition.x - horizontalRange)
        {
            direction = 1;
        }
        
        // إطلاق النار
        timeSinceLastFire += Time.deltaTime;
        if (timeSinceLastFire >= fireRate)
        {
            Fire();
            timeSinceLastFire = 0f;
        }
    }

    private void Fire()
    {
        if (enemyProjectilePrefab != null && firePoint != null && playerTransform != null)
        {
            GameObject proj = Instantiate(enemyProjectilePrefab, firePoint.position, Quaternion.identity);
            
            // اجعل الرصاصة تتجه نحو اللاعب
            Vector2 directionToPlayer = (playerTransform.position - firePoint.position).normalized;
            proj.GetComponent<Rigidbody2D>().linearVelocity = directionToPlayer * 10f; // سرعة الرصاصة 10
        }
    }
}