using System.Collections;
using UnityEngine;

[RequireComponent(typeof(StatsManager))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAI : MonoBehaviour
{
    private EnemyData_SO enemyData;

    private Transform playerTransform; // لتتبعه
    private float sinWaveFrequency = 2f;
    private float sinWaveAmplitude = 1.5f;
    private Vector3 startPos;
    private float nextFireTime;


    public void Initialize(EnemyData_SO data)
    {
        this.enemyData = data;

        // تطبيق البيانات فورًا
        GetComponent<SpriteRenderer>().sprite = enemyData.enemySprite;
        GetComponent<StatsManager>().Initialize(enemyData.stats); // ستحتاج لإضافة هذه الدالة أيضًا!

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPos = transform.position;
        nextFireTime = Time.time + Random.Range(0, enemyData.fireRate);
    }
    
    void Start()
    {
        // تطبيق البيانات من الـ ScriptableObject
        GetComponent<SpriteRenderer>().sprite = enemyData.enemySprite;
        // ملاحظة: StatsManager يجب أن يتمكن من أخذ SO وتطبيقه أيضًا

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPos = transform.position;
        nextFireTime = Time.time + Random.Range(0, enemyData.fireRate); // إطلاق نار عشوائي
    }

    void Update()
    {
        if (enemyData == null)
    {
        return; 
    }
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        switch (enemyData.movementType)
        {
            case EnemyMovementType.StraightDown:
                transform.Translate(Vector2.down * enemyData.speed * Time.deltaTime);
                break;

            case EnemyMovementType.SinWave:
                float x = Mathf.Sin(Time.time * sinWaveFrequency) * sinWaveAmplitude;
                transform.position = startPos + new Vector3(x, 0, 0) + (Vector3.down * enemyData.speed * Time.time);
                break;

            case EnemyMovementType.Chaser:
                // <<< تحسين: تحقق إذا كان اللاعب لا يزال موجودًا
                if (playerTransform != null)
                {
                    Vector3 direction = (playerTransform.position - transform.position).normalized;
                    transform.Translate(direction * enemyData.speed * Time.deltaTime);
                }
                else
                {
                    // السلوك البديل: تحرك للأسفل
                    transform.Translate(Vector2.down * enemyData.speed * Time.deltaTime);
                }
                break;
        }
    }

    private void HandleShooting()
    {
        if (enemyData.projectilePrefab != null && Time.time >= nextFireTime)
        {
            Instantiate(enemyData.projectilePrefab, transform.position, Quaternion.identity);
            nextFireTime = Time.time + enemyData.fireRate;
        }
    }

    

    
}