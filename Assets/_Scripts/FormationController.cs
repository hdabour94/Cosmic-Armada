using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float horizontalSpeed = 3f;
    [SerializeField] private float verticalStep = 0.5f; // المسافة التي ينزلها السرب
    [SerializeField] private float screenEdgePadding = 1f; // هامش حافة الشاشة

    [Header("Attacks")]
    [SerializeField] private float attackInterval = 2f; // كل كم ثانية يهاجم عدو
    
    private List<Transform> formationPoints = new List<Transform>();
    private List<FormationEnemyAI> activeEnemies = new List<FormationEnemyAI>();
    private int moveDirection = 1; // 1 لليمين، -1 لليسار
    private Camera mainCamera;
    private float minX, maxX;

    void Awake()
    {
        // جمع كل نقاط التشكيل الأبناء
        foreach (Transform point in transform)
        {
            formationPoints.Add(point);
        }
        mainCamera = Camera.main;
    }

    void Start()
    {
        // حساب حدود الشاشة
        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 10));
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 10));
        minX = leftEdge.x + screenEdgePadding;
        maxX = rightEdge.x - screenEdgePadding;

        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        // حركة السرب الأفقية
        transform.Translate(Vector3.right * moveDirection * horizontalSpeed * Time.deltaTime);

        // التحقق من الوصول إلى حافة الشاشة
        if ((transform.position.x > maxX && moveDirection > 0) || (transform.position.x < minX && moveDirection < 0))
        {
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        moveDirection *= -1; // عكس الاتجاه
        transform.position += Vector3.down * verticalStep; // النزول خطوة للأسفل
    }

    

    public void AssignEnemiesToFormation(List<GameObject> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (i >= formationPoints.Count) break; // توقف إذا كان عدد الأعداء أكثر من النقاط

            FormationEnemyAI enemyAI = enemies[i].GetComponent<FormationEnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.AssignToPoint(formationPoints[i], this); // <<< تمرير this
                activeEnemies.Add(enemyAI);
            }
        }
    }
    
     private IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);

            // <<< تحسين: تنظيف القائمة وإيقاف الهجوم إذا لم يبق أعداء
            activeEnemies.RemoveAll(e => e == null);
            if (activeEnemies.Count == 0)
            {
                yield break; // أوقف الكوروتين
            }
            
            int randomIndex = Random.Range(0, activeEnemies.Count);
            activeEnemies[randomIndex].StartAttack();
        }
    }

    public void RemoveEnemy(FormationEnemyAI enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }
}
