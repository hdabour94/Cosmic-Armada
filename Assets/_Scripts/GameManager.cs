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
        ///
        // --- منطق السرب الجديد ---
        if (waveData.formationPrefab != null)
        {
            // 1. أنشئ قائد السرب (التشكيل)
            //  GameObject formationObject = Instantiate(waveData.formationPrefab, transform.position, Quaternion.identity);
            GameObject formationObject = Instantiate(waveData.formationPrefab, new Vector3(-25, 40, 0), Quaternion.identity);

            FormationController formationController = formationObject.GetComponent<FormationController>();

            // 2. أنشئ الأعداء اللازمين للتشكيل
            List<GameObject> spawnedEnemies = new List<GameObject>();
            foreach (var enemyInfo in waveData.enemiesToSpawn)
            {
                // لا نحدد مكان العدو، سنضعه في (0,0,0) مؤقتًا
                GameObject newEnemy = Instantiate(GetEnemyBasePrefab(), Vector3.zero, Quaternion.identity);

                // تأكد من أن العدو لديه سكربت FormationEnemyAI
                if (newEnemy.GetComponent<FormationEnemyAI>() == null)
                {
                    newEnemy.AddComponent<FormationEnemyAI>();
                }

                // تهيئة الإحصائيات (إذا لزم الأمر)
                newEnemy.GetComponent<StatsManager>().Initialize(enemyInfo.enemyData.stats);

                spawnedEnemies.Add(newEnemy);
                activeEnemies.Add(newEnemy); // أضفهم للقائمة العامة لتتبع نهاية الموجة
            }

            // 3. عين الأعداء للتشكيل
            formationController.AssignEnemiesToFormation(spawnedEnemies);

            // الآن انتظر حتى يتم تدمير جميع الأعداء
            while (activeEnemies.Count > 0)
            {
                activeEnemies.RemoveAll(item => item == null);
                yield return null;
            }

            // دمر كائن التشكيل بعد انتهاء الموجة
            Destroy(formationObject);
        }
        else // --- المنطق القديم للموجات العشوائية ---
        {
            // ... (الكود السابق للموجات العشوائية يبقى هنا) ...
        
         Debug.Log("Spawning Wave " + (currentWaveIndex + 1));
        isSpawning = true;
        
        // إعادة تعيين عدد الأعداء المولودين للموجة الجديدة
        waveEnemiesSpawned = waveData.enemiesToSpawn.Length;

        foreach (var enemyInfo in waveData.enemiesToSpawn)
        {
            yield return new WaitForSeconds(enemyInfo.delayBeforeSpawn);
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // قمنا بتغيير طفيف هنا لتسهيل إضافة العدو للقائمة
            GameObject enemyObject = GetEnemyBasePrefab(); // احصل على الـ Prefab
            GameObject newEnemy = Instantiate(enemyObject, randomSpawnPoint.position, Quaternion.identity);
            
            newEnemy.GetComponent<EnemyAI>().Initialize(enemyInfo.enemyData);
            
            // أضف العدو الجديد إلى قائمتنا لتتبعه
            activeEnemies.Add(newEnemy);
        }

        isSpawning = false;
        Debug.Log("Wave " + (currentWaveIndex + 1) + " finished spawning. Now waiting for completion...");

        // --- الجزء الجديد: حلقة الانتظار ---
        while (activeEnemies.Count > 0)
        {
            // قم بتنظيف القائمة من أي أعداء تم تدميرهم (إذا حدث خطأ ما ولم يتم إزالتهم)
            activeEnemies.RemoveAll(item => item == null);
            
            // انتظر إطارًا واحدًا ثم تحقق مرة أخرى
            yield return null; 
        }

        Debug.Log("Wave " + (currentWaveIndex + 1) + " COMPLETED!");
    }

        /// 
       
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
        // UIManager.Instance.UpdateCoinsText(coins); // يجب إضافة هذه الدالة للواجهة
    }
    private GameObject GetEnemyBasePrefab()
    {
        // يفترض أن يكون لديك prefab واحد اسمه "BaseEnemy" يحتوي على سكربت EnemyAI
        // وSpriteRenderer وCollider... إلخ.
        return Resources.Load<GameObject>("Prefabs/BaseEnemy"); // تأكد من وجوده في مجلد Resources/_Prefabs
    }




    public void EndLevel(bool victory)
    {
        StopAllCoroutines();
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

        // التحديث الأولي للواجهة بعد التحميل
        // يتم استدعاؤه في Start في UIManager لضمان أن كل شيء جاهز
    }
    #endregion
}