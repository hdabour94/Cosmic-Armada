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
            weapon.InitializeFromData(enemyData); // أنشئ دالة تهيئة جديدة في EnemyWeapon 

        }
    }

    private void SetMovementBehavior()
    {
        Transform playerTransform = LevelManager.Instance?.CurrentPlayerInstance?.transform;

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
    private void Update()
    {
        // تحقق من وجود سلوك حركة ثم قم بتنفيذه
        if (movementBehavior != null)
        {
            movementBehavior.Move(this.transform);
        }
    }
}