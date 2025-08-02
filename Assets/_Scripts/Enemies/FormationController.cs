using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float horizontalSpeed = 2.5f; // تم إصلاح التحذير السابق هنا
    [SerializeField] private float verticalStep = 0.5f;
    [SerializeField] private float screenEdgePadding = 1.0f;

    [Header("Attacks")]
    [SerializeField] private float attackInterval = 1.5f;

    // --- سطر التعريف المفقود أو الذي تم تغييره ---
    // هذا هو المتغير الذي يجب أن يكون موجودًا على مستوى الكلاس
    private List<Transform> formationPoints = new List<Transform>();
    // ---------------------------------------------

    private List<FormationEnemyAI> activeEnemies = new List<FormationEnemyAI>();
    private int moveDirection = 1;
    private Camera mainCamera;
    private float minX, maxX;

    void Awake()
    {
        // جمع كل نقاط التشكيل الأبناء وتخزينها في القائمة
        foreach (Transform point in transform)
        {
            if (point != this.transform) // تأكد من عدم إضافة المتحكم نفسه
            {
                formationPoints.Add(point);
            }
        }
        mainCamera = Camera.main;
    }

    void Start()
    {
        // حساب حدود الشاشة
        if (mainCamera != null)
        {
            Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 10));
            Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 10));
            minX = leftEdge.x + screenEdgePadding;
            maxX = rightEdge.x - screenEdgePadding;
        }

        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        // استخدام المتغير horizontalSpeed لإصلاح التحذير السابق
        transform.Translate(Vector3.right * moveDirection * horizontalSpeed * Time.deltaTime);

        if ((transform.position.x > maxX && moveDirection > 0) || (transform.position.x < minX && moveDirection < 0))
        {
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        moveDirection *= -1;
        transform.position += Vector3.down * verticalStep;
    }
    
    // هذه هي الدالة التي حدث فيها الخطأ (السطر 63 تقريبًا)
    public void AssignEnemies(List<FormationEnemyAI> enemies)
    {
        if (enemies == null || enemies.Count == 0) return;

        // --- هنا كان الخطأ ---
        // تأكد من أنك تستخدم الاسم الصحيح "formationPoints"
        if (formationPoints.Count == 0)
        {
            Debug.LogError("FormationAnchor has no formation points (children) assigned!");
            return;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (i >= formationPoints.Count) break;

            FormationEnemyAI currentEnemyAI = enemies[i];
            Transform targetPoint = formationPoints[i]; // <<<--- السطر 63

            currentEnemyAI.transform.SetParent(targetPoint);
            currentEnemyAI.transform.localPosition = Vector3.zero;
            currentEnemyAI.transform.localRotation = Quaternion.identity;

            currentEnemyAI.AssignToPoint(targetPoint, this);

            if (!activeEnemies.Contains(currentEnemyAI))
            {
                activeEnemies.Add(currentEnemyAI);
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            
            activeEnemies.RemoveAll(e => e == null);
            if (activeEnemies.Count == 0) yield break;
            
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