// START OF FILE EnemyAI.txt
﻿using UnityEngine;

[RequireComponent(typeof(StatsManager), typeof(SpriteRenderer))]
public class EnemyAI : MonoBehaviour
{
    private EnemyData_SO enemyData;
    private IMovementBehavior movementBehavior;
    private Transform playerTransform;

    public void Initialize(EnemyData_SO data)
    {
        this.enemyData = data;
        GetComponent<SpriteRenderer>().sprite = enemyData.enemySprite;
        GetComponent<StatsManager>().Initialize(enemyData.stats); 
        
        SetMovementBehavior();
        
        if (enemyData.projectilePrefab != null)
        {
            var weapon = gameObject.AddComponent<EnemyWeapon>();
            
            // <--- التعديل هنا: قم بتهيئة كائن settings نفسه أولاً
            weapon.settings = new WeaponBase.WeaponSettings(); 

            // هذه الأسطر ستعمل الآن لأن 'settings' لم تعد null
            weapon.settings.projectilePrefab = enemyData.projectilePrefab; 
            weapon.settings.firePoint = transform; // يمكن أن يكون هذا هو firePoint الافتراضي للعدو (مركز الكائن)
            weapon.settings.fireRate = enemyData.fireRate; 
        }
    }

    private void SetMovementBehavior()
    {
        playerTransform = LevelManager.Instance?.CurrentPlayerInstance?.transform;

        switch (enemyData.movementType)
        {
            case EnemyMovementType.StraightDown:
                movementBehavior = new StraightMovement(enemyData.speed);
                break;
            case EnemyMovementType.SinWave:
                movementBehavior = new SinWaveMovement(enemyData.speed, 2f, 1.5f);
                break;
            case EnemyMovementType.Chaser:
                movementBehavior = new ChaseMovement(enemyData.speed, playerTransform);
                break;
        }
    }
}