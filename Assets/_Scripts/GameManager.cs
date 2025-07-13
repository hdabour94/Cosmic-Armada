using System.Collections;
using System.Collections.Generic; // <<<--- أضف هذا السطر لاستخدام القوائم
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton

    [Header("Level Configuration")]
    [SerializeField] private LevelData_SO currentLevelData; // اسحب ملف بيانات المرحلة هنا

    // --- قسم جديد لإعدادات اللاعب ---
    [Header("Player Configuration")]
    [Tooltip("المركبة التي سيبدأ بها اللاعب.")]
    [SerializeField] private GameObject playerPrefab;
    [Tooltip("المكان الذي سيظهر فيه اللاعب عند بدء المرحلة.")]
    [SerializeField] private Transform playerSpawnPoint;
    [Tooltip("الإحصائيات المبدئية لمركبة اللاعب.")]
    [SerializeField] private CharacterStats_SO playerStartingStats;
    // ------------------------------------

    [Header("Spawning")]
    [SerializeField] private Transform[] spawnPoints; // نقاط توليد الأعداء
    [Tooltip("الـ Prefab الأساسي الذي يحتوي على كل السكربتات اللازمة للعدو")]
    [SerializeField] private GameObject baseEnemyPrefab; // <<< تحسين: استبدال Resources.Load

    [Header("Player State")]
    public int playerLevel = 1;
    public int playerXP = 0;
    public int xpToNextLevel = 100;
    public int coins = 0;

        public GameObject CurrentPlayerInstance { get; private set; }

    private int currentWaveIndex = 0;
    private bool isSpawning = false; // لمنع بدء موجة جديدة أثناء وجود موجة حالية
     private List<GameObject> activeEnemies = new List<GameObject>(); // قائمة لتتبع الأعداء الأحياء
    private int waveEnemiesSpawned = 0;
    // عدد الأعداء الذين تم توليدهم في الموجة الحالية
    // ------------------------------------

    // ... (المتغيرات الأخرى) ...
   
    
    public void SetCurrentLevel(LevelData_SO levelData)
    {
        this.currentLevelData = levelData;
    }
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

     public void InitializeLevel()
    {
        LoadGame();      // تحميل بيانات تقدم اللاعب (المستوى، العملات)
        SpawnPlayer();   // توليد وتهيئة مركبة اللاعب
        StartLevel();    // بدء تسلسل موجات الأعداء
    }

    



    void Start()
    {
        InitializeLevel();
    }

    private GameObject GetEnemyBasePrefab()
    {
        // <<< تحسين: الآن نستخدم المتغير المسند من الـ Inspector مباشرة
        if (baseEnemyPrefab == null)
        {
            Debug.LogError("Base Enemy Prefab is not assigned in GameManager Inspector!");
            return null;
        }
        return baseEnemyPrefab;
    }

    public void EndLevel(bool victory)
    {
        if (victory)
        {
            Debug.Log("LEVEL COMPLETE! YOU WIN!");
            // لاحقًا: عرض شاشة النصر
        }
        else
        {
            Debug.Log("GAME OVER! YOU LOST!");
            // لاحقًا: عرض شاشة الهزيمة
        }
        
        // <<< تحسين: إيقاف الوقت لتجميد اللعبة عند نهايتها
        Time.timeScale = 0f;
        StopAllCoroutines();
    }

     // --- دالة جديدة لتوليد وتهيئة اللاعب ---
    void SpawnPlayer()
    {
        if (playerPrefab == null || playerSpawnPoint == null)
        {
            Debug.LogError("Player Prefab or Spawn Point is not set in GameManager!");
            return;
        }

        if (playerStartingStats == null)
        {
            Debug.LogError("Player Starting Stats are not set in GameManager!");
            return;
        }

        // 1. أنشئ نسخة من اللاعب في مكان البداية
        CurrentPlayerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);

        // 2. احصل على مكون StatsManager من النسخة التي تم إنشاؤها
        StatsManager playerStatsManager = CurrentPlayerInstance.GetComponent<StatsManager>();

        // 3. قم بتهيئة StatsManager باستخدام ملف الإحصائيات الذي حددناه
        if (playerStatsManager != null)
        {
            // هنا تحدث الديناميكية!
            // لاحقًا، يمكن أن تأتي playerStartingStats من اختيار اللاعب في القائمة الرئيسية
            playerStatsManager.Initialize(playerStartingStats);
            Debug.Log($"Player '{CurrentPlayerInstance.name}' spawned and initialized with stats from '{playerStartingStats.name}'.");
        }
        else
        {
            Debug.LogError("Player Prefab does not have a StatsManager component!");
        }
    }
    // ------------------------------------

    private IEnumerator LevelSequenceRoutine()
    {
        for (int i = 0; i < currentLevelData.waves.Length; i++)
        {
            currentWaveIndex = i;
            WaveData_SO wave = currentLevelData.waves[currentWaveIndex];

            UIManager.Instance.ShowNotification("Wave " + (currentWaveIndex + 1));
            yield return new WaitForSeconds(1.5f);

            // سنستدعي الآن دالة جديدة لتوليد وانتظار الموجة
            yield return StartCoroutine(SpawnAndWaitForWaveToEnd(wave)); // <<<--- تغيير هنا

            // انتظر قليلاً قبل بدء الموجة التالية لإعطاء اللاعب فرصة لالتقاط الأنفاس
            yield return new WaitForSeconds(3f);
        }

        yield return StartCoroutine(SpawnBossRoutine());
    }

    
    // هذه الدالة الجديدة تجمع بين التوليد والانتظار
    private IEnumerator SpawnAndWaitForWaveToEnd(WaveData_SO waveData)
{
    isSpawning = true;
    
    // --- منطق السرب (Formation) ---
    if (waveData.formationPrefab != null)
    {
        Debug.Log("Spawning FORMATION wave: " + waveData.name);
        
        // 1. أنشئ قائد السرب
        GameObject formationObject = Instantiate(waveData.formationPrefab, transform.position, Quaternion.identity);
        FormationController formationController = formationObject.GetComponent<FormationController>();

        // 2. أنشئ الأعداء اللازمين للتشكيل
        List<GameObject> spawnedEnemies = new List<GameObject>();
        foreach (var enemyInfo in waveData.enemiesToSpawn)
        {
            GameObject newEnemy = Instantiate(GetEnemyBasePrefab(), Vector3.zero, Quaternion.identity);
            
            // تهيئة السكربتات الضرورية
            if (newEnemy.GetComponent<FormationEnemyAI>() == null) newEnemy.AddComponent<FormationEnemyAI>();
            newEnemy.GetComponent<StatsManager>().Initialize(enemyInfo.enemyData.stats);
            
            spawnedEnemies.Add(newEnemy);
            activeEnemies.Add(newEnemy);
        }

        // 3. عين الأعداء للتشكيل
        if(formationController != null)
        {
            formationController.AssignEnemiesToFormation(spawnedEnemies);
        }
        else
        {
            Debug.LogError("The assigned Formation Prefab is missing the FormationController script!");
        }
    }
    // --- منطق الموجات العشوائية (الحل للخطأ) ---
    else
    {
        Debug.Log("Spawning RANDOM wave: " + waveData.name);

        foreach (var enemyInfo in waveData.enemiesToSpawn)
        {
            yield return new WaitForSeconds(enemyInfo.delayBeforeSpawn);
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            GameObject newEnemy = Instantiate(GetEnemyBasePrefab(), randomSpawnPoint.position, Quaternion.identity);
            
            // استخدم EnemyAI العادي هنا وليس FormationEnemyAI
            if (newEnemy.GetComponent<EnemyAI>() == null) newEnemy.AddComponent<EnemyAI>();
            newEnemy.GetComponent<EnemyAI>().Initialize(enemyInfo.enemyData);
            
            activeEnemies.Add(newEnemy);
        }
    }

    isSpawning = false;
    Debug.Log("Wave finished spawning. Waiting for all enemies to be defeated...");

    // حلقة الانتظار الموحدة لكلا نوعي الموجات
    while (activeEnemies.Count > 0)
    {
        activeEnemies.RemoveAll(item => item == null);
        yield return null; 
    }

    Debug.Log("Wave COMPLETED!");
}
     // --- دالة عامة لإزالة الأعداء من القائمة ---
    // سيتم استدعاؤها من العدو نفسه عند موته
    public void OnEnemyDestroyed(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }


    private void StartLevel()
    {
        currentWaveIndex = 0;
        StartCoroutine(LevelSequenceRoutine());
    }
    

    private IEnumerator SpawnWaveRoutine(WaveData_SO waveData)
    {
        Debug.Log("Spawning Wave " + (currentWaveIndex + 1));
        isSpawning = true;

        foreach (var enemyInfo in waveData.enemiesToSpawn)
        {
            // انتظر التأخير المحدد قبل توليد العدو
            yield return new WaitForSeconds(enemyInfo.delayBeforeSpawn);

            // اختر نقطة توليد عشوائية
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // أنشئ عدوًا فارغًا وقم بإعداده
            // ملاحظة: من الأفضل أن يكون لديك Prefab أساسي للعدو يحتوي على سكربت EnemyAI
            // ثم تقوم بتمرير EnemyData_SO إليه.
            GameObject enemyObject = Instantiate(GetEnemyBasePrefab(), randomSpawnPoint.position, Quaternion.identity);
            enemyObject.GetComponent<EnemyAI>().Initialize(enemyInfo.enemyData); // سنحتاج لإضافة دالة Initialize
        }

        isSpawning = false;
        Debug.Log("Wave " + (currentWaveIndex + 1) + " finished spawning.");
    }

    private IEnumerator SpawnBossRoutine()
    {
        Debug.Log("Spawning Boss...");
        UIManager.Instance.ShowNotification("WARNING: BOSS INCOMING!");
        yield return new WaitForSeconds(currentLevelData.timeBeforeBoss);

        if (currentLevelData.bossPrefab != null)
        {
            // توليد الزعيم في نقطة محددة (أو عشوائية)
            Instantiate(currentLevelData.bossPrefab, spawnPoints[0].position, Quaternion.identity);
        }
    }





    private void OnApplicationQuit()
    {
        SaveGame();
    }
    
    

    ////

    public void AddXP(int xpAmount)
    {
        playerXP += xpAmount;
        UIManager.Instance.UpdateXPUI(playerXP, xpToNextLevel);
        if (playerXP >= xpToNextLevel) LevelUp();
    }

    private void LevelUp()
    {
        playerLevel++;
        playerXP -= xpToNextLevel;
        xpToNextLevel = (int)(xpToNextLevel * 1.5f);
        UIManager.Instance.UpdateLevelText(playerLevel);
        UIManager.Instance.UpdateXPUI(playerXP, xpToNextLevel);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UIManager.Instance.UpdateCoinText(coins); // يجب إضافة هذه الدالة للواجهة
    }
    


    #region Save & Load System
    public void SaveGame()
    {
        PlayerData data = new PlayerData
        {
            playerLevel = this.playerLevel,
            playerXP = this.playerXP,
            xpToNextLevel = this.xpToNextLevel,
            coins = this.coins
        };
        SaveSystem.SavePlayer(data);
    }

    public void LoadGame()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        this.playerLevel = data.playerLevel;
        this.playerXP = data.playerXP;
        this.xpToNextLevel = data.xpToNextLevel;
        this.coins = data.coins;

        Debug.Log($"Player data loaded. Level: {this.playerLevel}");

        UIManager.Instance.UpdateCoinText(coins);
        UIManager.Instance.UpdateLevelText(playerLevel);
        UIManager.Instance.UpdateXPUI(playerXP, xpToNextLevel);
        // التحديث الأولي للواجهة بعد التحميل
        // يتم استدعاؤه في Start في UIManager لضمان أن كل شيء جاهز
    }
    #endregion
}