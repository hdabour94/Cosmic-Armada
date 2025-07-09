using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyFormationManager : MonoBehaviour
{
   /* [SerializeField] private FormationData_SO formationData;

    private List<FormationEnemyAI> activeEnemies = new List<FormationEnemyAI>();
    private Dictionary<FormationEnemyAI, Vector3> originalPositions = new Dictionary<FormationEnemyAI, Vector3>();
    private Vector3 initialPosition;
    private int moveDirection = 1;
    private float timeSinceLastAttack = 0f;

    void Start()
    {
        initialPosition = transform.position;
        SpawnFormation();
        timeSinceLastAttack = formationData.timeBetweenAttacks; // ابدأ جاهزًا للهجوم
    }

    void Update()
    {
        MoveFormation();
        HandleAttacks();
    }

    private void SpawnFormation()
    {
        for (int row = 0; row < formationData.rows; row++)
        {
            for (int col = 0; col < formationData.columns; col++)
            {
                // حساب موقع العدو بالنسبة لمدير التشكيل
                float xPos = col * formationData.horizontalSpacing;
                float yPos = row * -formationData.verticalSpacing; // سالب للأسفل
                Vector3 spawnPos = new Vector3(xPos, yPos, 0);

                GameObject enemyInstance = Instantiate(formationData.enemyPrefab, transform);
                enemyInstance.transform.localPosition = spawnPos;

                FormationEnemyAI enemyAI = enemyInstance.GetComponent<FormationEnemyAI>();
                if (enemyAI != null)
                {
                    // تهيئة العدو وتخزين بياناته
                    enemyAI.Initialize(this, enemyInstance.transform.localPosition);
                    activeEnemies.Add(enemyAI);
                    originalPositions.Add(enemyAI, enemyInstance.transform.localPosition);
                }
            }
        }
    }

    private void MoveFormation()
    {
        transform.Translate(Vector3.right * moveDirection * formationData.formationMoveSpeed * Time.deltaTime);

        // تغيير الاتجاه عند الوصول للحدود
        if (transform.position.x > initialPosition.x + formationData.moveBounds ||
            transform.position.x < initialPosition.x - formationData.moveBounds)
        {
            moveDirection *= -1;
        }
    }

    private void HandleAttacks()
    {
        timeSinceLastAttack += Time.deltaTime;
        if (timeSinceLastAttack >= formationData.timeBetweenAttacks)
        {
            // ابحث عن أعداء متاحين للهجوم (الذين هم في التشكيل)
            var availableAttackers = activeEnemies.Where(e => e.currentState == FormationEnemyAI.EnemyState.InFormation).ToList();

            if (availableAttackers.Any())
            {
                // اختر عدوًا عشوائيًا من المتاحين وأمره بالهجوم
                int randomIndex = Random.Range(0, availableAttackers.Count);
                availableAttackers[randomIndex].StartChase(formationData.chaseDuration);
                timeSinceLastAttack = 0f; // إعادة ضبط المؤقت
            }
        }
    }

    public void ReportEnemyDestroyed(FormationEnemyAI enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        // إذا تم تدمير كل الأعداء، دمر مدير التشكيل نفسه
        if (activeEnemies.Count == 0)
        {
            Debug.Log("Formation Destroyed!");
            // لاحقًا: يمكن إبلاغ GameManager لإنهاء المرحلة أو استدعاء الزعيم
            Destroy(gameObject);
        }
    }*/
}