// START OF FILE LevelManager.txt
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private LevelData_SO currentLevelData;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject baseEnemyPrefab;
    [SerializeField] private GameObject defaultPlayerPrefab;

    public GameObject CurrentPlayerInstance { get; private set; }

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() => InitializeLevel();

    public void InitializeLevel()
    {
        SpawnPlayer();
        StartCoroutine(LevelSequenceRoutine());
    }

    private void SpawnPlayer()
{
    ShipData_SO selectedShipData = null;
    GameObject prefabToSpawn = null;

    // 1. حاول الحصول على المركبة المختارة من القائمة الرئيسية
    if (GameDataHolder.Instance != null && GameDataHolder.Instance.SelectedShip != null)
    {
        selectedShipData = GameDataHolder.Instance.SelectedShip;
        prefabToSpawn = selectedShipData.prefab;
        Debug.Log($"Spawning selected ship: {selectedShipData.shipName}");
    }
    else
    {
        // 2. إذا لم تكن هناك مركبة مختارة، استخدم المركبة الافتراضية
        prefabToSpawn = defaultPlayerPrefab;
        Debug.LogWarning("No ship selected from menu. Spawning default player.");
    }

    // 3. تحقق من أن هناك Prefab صالح للإنشاء
    if (prefabToSpawn == null)
    {
        Debug.LogError("FATAL: Prefab to spawn is null! Cannot create player. Check ShipData_SO and defaultPlayerPrefab assignment in LevelManager.");
        return;
    }

    // 4. أنشئ اللاعب
    CurrentPlayerInstance = Instantiate(prefabToSpawn, playerSpawnPoint.position, Quaternion.identity);

    // 5. احصل على سكربت التهيئة المركزي
    Player playerComponent = CurrentPlayerInstance.GetComponent<Player>();
    if (playerComponent == null)
    {
        Debug.LogError($"The spawned player prefab '{CurrentPlayerInstance.name}' is MISSING the central 'Player.cs' script!", CurrentPlayerInstance);
        return;
    }

    // 6. قم بالتهيئة فقط إذا كانت هناك بيانات مركبة مختارة
    // (اللاعب الافتراضي قد يكون مهيئًا مسبقًا في الـ Prefab الخاص به)
    if (selectedShipData != null)
    {
        playerComponent.Initialize(selectedShipData);
    }
}

    private IEnumerator LevelSequenceRoutine()
    {
        for (int i = 0; i < currentLevelData.waves.Length; i++)
        {
            currentWaveIndex = i;
            var wave = currentLevelData.waves[currentWaveIndex];
            
            UIManager.Instance?.ShowNotification($"Wave {currentWaveIndex + 1}");
            yield return new WaitForSeconds(1.5f);
            
            yield return StartCoroutine(SpawnAndWaitForWaveToEnd(wave));
            yield return new WaitForSeconds(3f);
        }
        
        yield return StartCoroutine(SpawnBossRoutine());
    }

    // في LevelManager.cs

    /*
        private IEnumerator SpawnAndWaitForWaveToEnd(WaveData_SO waveData)
        {
            // التأكد من أن قائمة الأعداء فارغة قبل بدء الموجة
            activeEnemies.Clear();
            isSpawning = true;
            Debug.Log($"Starting wave '{waveData.name}'. Waiting for completion...");

            // -----------------------------------------------------------------
            // الجزء 1: توليد الأعداء (Spawning)
            // -----------------------------------------------------------------

            if (waveData.formationPrefab != null)
            {
                // --- منطق موجة التشكيل (Formation Wave) ---
                Debug.Log("Wave type: Formation");
                GameObject formationObject = Instantiate(waveData.formationPrefab, transform.position, Quaternion.identity);
                FormationController controller = formationObject.GetComponent<FormationController>();

                if (controller != null)
                {
                    List<FormationEnemyAI> enemiesForFormation = new List<FormationEnemyAI>();
                    foreach (var enemyInfo in waveData.enemiesToSpawn)
                    {
                        // أنشئ العدو في (0,0,0) مؤقتًا، سيقوم التشكيل بتحديد موقعه
                        GameObject enemyGO = Instantiate(baseEnemyPrefab, this.transform.position, Quaternion.identity);

                        FormationEnemyAI enemyAI = enemyGO.GetComponent<FormationEnemyAI>();
                        if (enemyAI != null)
                        {
                            enemyAI.Initialize(enemyInfo.enemyData);
                            enemiesForFormation.Add(enemyAI);
                            activeEnemies.Add(enemyGO); // أضف كائن اللعبة إلى القائمة التي يتم تتبعها
                        }
                        else
                        {
                            Debug.LogError($"BaseEnemyPrefab is missing the FormationEnemyAI component. Cannot create formation enemy from '{enemyInfo.enemyData.name}'.");
                            Destroy(enemyGO);
                        }
                    }
                    controller.AssignEnemies(enemiesForFormation);
                }
                else
                {
                    Debug.LogError("The assigned Formation Prefab is missing the FormationController script!");
                }
            }
            else
            {
                // --- منطق الموجة العشوائية (Random Wave) ---
                Debug.Log("Wave type: Random");
                foreach (var enemyInfo in waveData.enemiesToSpawn)
                {
                    // انتظر التأخير المحدد *قبل* إنشاء العدو
                    if (enemyInfo.delayBeforeSpawn > 0)
                    {
                        yield return new WaitForSeconds(enemyInfo.delayBeforeSpawn);
                    }

                    Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    GameObject enemyGO = Instantiate(baseEnemyPrefab, point.position, Quaternion.identity);

                    EnemyAI enemyAI = enemyGO.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.Initialize(enemyInfo.enemyData);
                        activeEnemies.Add(enemyGO); // أضف كائن اللعبة إلى القائمة التي يتم تتبعها
                    }
                    else
                    {
                        Debug.LogError($"BaseEnemyPrefab is missing the EnemyAI component. Cannot create random enemy from '{enemyInfo.enemyData.name}'.");
                        Destroy(enemyGO);
                    }
                }
            }

            isSpawning = false;
            Debug.Log($"Spawning complete for wave. {activeEnemies.Count} enemies are active. Now waiting...");

            // -----------------------------------------------------------------
            // الجزء 2: الانتظار (Waiting)
            // -----------------------------------------------------------------

            // تأكد من وجود أعداء تم توليدهم قبل الدخول في حلقة الانتظار
            if (activeEnemies.Count == 0)
            {
                Debug.LogWarning("Wave ended immediately because no enemies were spawned successfully.");
                yield break; // اخرج من الكوروتين إذا لم يتم إنشاء أي أعداء
            }

            // انتظر حتى يتم تدمير جميع الأعداء في القائمة
            while (activeEnemies.Count > 0)
            {
                // قم بإزالة أي كائنات أصبحت null (تم تدميرها) من القائمة
                activeEnemies.RemoveAll(enemy => enemy == null);
                yield return null; // انتظر الإطار التالي ثم تحقق مرة أخرى
            }

            Debug.Log($"Wave '{waveData.name}' COMPLETED!");
        }
    */
private IEnumerator SpawnAndWaitForWaveToEnd(WaveData_SO waveData)
{
isSpawning = true;
      
if (waveData.formationPrefab != null)
    {
        // توليد التشكيلات
        var formation = Instantiate(waveData.formationPrefab);
        var controller = formation.GetComponent<FormationController>();
        List<FormationEnemyAI> enemies = new(); 
        
        foreach (var enemyInfo in waveData.enemiesToSpawn)
        {
            var enemyGO = Instantiate(baseEnemyPrefab); 
            var enemyAI = enemyGO.GetComponent<FormationEnemyAI>(); 
            if (enemyAI != null) 
            {
                enemyAI.Initialize(enemyInfo.enemyData); 
                enemies.Add(enemyAI); 
                activeEnemies.Add(enemyGO); 
            }
            else
            {
                Debug.LogError("BaseEnemyPrefab does not have FormationEnemyAI component for formation enemies!");
                Destroy(enemyGO); 
            }
        }
        
        controller?.AssignEnemies(enemies); 
    }
    else
    {
        // توليد عشوائي
        foreach (var enemyInfo in waveData.enemiesToSpawn)
        {
            yield return new WaitForSeconds(enemyInfo.delayBeforeSpawn);
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var enemy = Instantiate(baseEnemyPrefab, point.position, Quaternion.identity);
            enemy.GetComponent<EnemyAI>().Initialize(enemyInfo.enemyData);
            activeEnemies.Add(enemy);
        }
    }
    
    isSpawning = false;
    
    // انتظار انتهاء الموجة
    while (activeEnemies.Count > 0)
    {
        activeEnemies.RemoveAll(e => e == null);
        yield return null;
    }
}

    
    private IEnumerator SpawnBossRoutine()
    {
        UIManager.Instance?.ShowNotification("WARNING: BOSS INCOMING!");
        yield return new WaitForSeconds(currentLevelData.timeBeforeBoss);

        // هنا التعديل: استخدم prefab من BossData_SO مباشرة
        if (currentLevelData.bossData?.prefab != null)
        {
            GameObject bossGO = Instantiate(
                currentLevelData.bossData.prefab, // <--- استخدم prefab من bossData
                spawnPoints[0].position,
                Quaternion.identity
            );

            StatsManager sm = bossGO.GetComponent<StatsManager>();
            if (sm != null) sm.Initialize(currentLevelData.bossData.characterStats); // <--- تأكد من استخدام characterStats

            BossAI bossAI = bossGO.GetComponent<BossAI>();
            if (bossAI != null)
            {
                bossAI.Initialize(currentLevelData.bossData); // <--- قم بتهيئة BossAI بـ BossData_SO الكامل
            }
        }
        else
        {
            Debug.LogError("BossData_SO or Boss Prefab is not assigned in currentLevelData for boss routine!");
        }
    }

    public void OnEnemyDestroyed(GameObject enemy) => activeEnemies.Remove(enemy);

    public void SetCurrentLevel(LevelData_SO levelData) => currentLevelData = levelData;

    public void EndLevel(bool victory)
    {
        Debug.Log(victory ? "VICTORY!" : "DEFEAT!");
        Time.timeScale = 0f;
        StopAllCoroutines();
         UIManager.Instance?.ShowLevelComplete(victory); // يمكنك تفعيل هذا لإظهار شاشة الفوز/الخسارة
    }
}