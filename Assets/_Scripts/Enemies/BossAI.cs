// START OF FILE BossAI.txt
﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class BossPattern
{
    public BossPatternType type;
    public int projectileCount;
}

[RequireComponent(typeof(StatsManager))]
public class BossAI : MonoBehaviour
{
    [SerializeField] private Transform[] movePoints;
    [SerializeField] private float timeBetweenPatterns = 3f;
    [SerializeField] private GameObject projectilePrefab;
    
    private BossData_SO bossData;
    private int currentPattern = 0;
    private BossPattern[] patterns = new BossPattern[]
    {
        new BossPattern { type = BossPatternType.CircularAttack, projectileCount = 12 },
        new BossPattern { type = BossPatternType.MoveToPoints, projectileCount = 0 }
    };

    public void Initialize(BossData_SO data)
{
    this.bossData = data;
    
    // استخدم data.stats بدلاً من data.characterStats إذا كان هذا اسم الحقل في ملفك
    // أو عدل ملف BossData_SO ليحتوي على الحقل characterStats كما في الخطوة 1
    GetComponent<StatsManager>().Initialize(data.characterStats /* Removed ?? data.stats */);
    
    StartCoroutine(BehaviorRoutine());
}

    private IEnumerator MoveToPoints(Transform[] points)
    {
        foreach (var point in points)
        {
            while (Vector3.Distance(transform.position, point.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, point.position, 5f * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator BehaviorRoutine()
    {
        while (true)
        {
            yield return ExecutePattern(currentPattern);
            currentPattern = (currentPattern + 1) % bossData.patterns.Length;
            yield return new WaitForSeconds(timeBetweenPatterns);
        }
    }

    private IEnumerator ExecutePattern(int patternIndex)
    {
        // تنفيذ نمط الهجوم حسب البيانات
        var pattern = bossData.patterns[patternIndex];
        
        switch (pattern.type)
        {
            case BossPatternType.CircularAttack:
                yield return StartCoroutine(CircularAttack(pattern.projectileCount));
                break;
            case BossPatternType.MoveToPoints:
                yield return StartCoroutine(MoveToPoints(movePoints));
                break;
        }
    }

    private IEnumerator CircularAttack(int projectileCount)
    {
        float angleStep = 360f / projectileCount;
        for (int i = 0; i < projectileCount; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, i * angleStep);
            Instantiate(bossData.projectilePrefab, transform.position, rotation);
        }
        yield return null;
    }
}

[System.Serializable]
public enum BossPatternType { CircularAttack, MoveToPoints }