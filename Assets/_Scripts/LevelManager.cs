using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private LevelData_SO currentLevelData;

    [Header("Player Configuration")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private CharacterStats_SO playerStartingStats;

    [Header("Spawning")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject baseEnemyPrefab;

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

    void Start()
    {
        InitializeLevel();
    }

    public void InitializeLevel()
    {
        SpawnPlayer();
        StartCoroutine(LevelSequenceRoutine());
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null || playerSpawnPoint == null || playerStartingStats == null)
        {
            Debug.LogError("Missing player spawn configuration in LevelManager!");
            return;
        }

        CurrentPlayerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        var stats = CurrentPlayerInstance.GetComponent<StatsManager>();
        if (stats != null)
        {
            stats.Initialize(playerStartingStats);
        }
    }

    private IEnumerator LevelSequenceRoutine()
    {
        for (int i = 0; i < currentLevelData.waves.Length; i++)
        {
            currentWaveIndex = i;
            var wave = currentLevelData.waves[currentWaveIndex];
            UIManager.Instance?.ShowNotification("Wave " + (currentWaveIndex + 1));
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(SpawnAndWaitForWaveToEnd(wave));
            yield return new WaitForSeconds(3f);
        }
        yield return StartCoroutine(SpawnBossRoutine());
    }

    private IEnumerator SpawnAndWaitForWaveToEnd(WaveData_SO waveData)
    {
        isSpawning = true;
        if (waveData.formationPrefab != null)
        {
            var formation = Instantiate(waveData.formationPrefab, transform.position, Quaternion.identity);
            var controller = formation.GetComponent<FormationController>();
            List<GameObject> enemies = new();

            foreach (var enemyInfo in waveData.enemiesToSpawn)
            {
                var enemy = Instantiate(baseEnemyPrefab, Vector3.zero, Quaternion.identity);
                if (!enemy.TryGetComponent<FormationEnemyAI>(out _)) enemy.AddComponent<FormationEnemyAI>();
                enemy.GetComponent<StatsManager>().Initialize(enemyInfo.enemyData.stats);
                enemies.Add(enemy);
                activeEnemies.Add(enemy);
            }

            controller?.AssignEnemiesToFormation(enemies);
        }
        else
        {
            foreach (var enemyInfo in waveData.enemiesToSpawn)
            {
                yield return new WaitForSeconds(enemyInfo.delayBeforeSpawn);
                Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
                var enemy = Instantiate(baseEnemyPrefab, point.position, Quaternion.identity);
                if (!enemy.TryGetComponent<EnemyAI>(out _)) enemy.AddComponent<EnemyAI>();
                enemy.GetComponent<EnemyAI>().Initialize(enemyInfo.enemyData);
                activeEnemies.Add(enemy);
            }
        }

        isSpawning = false;
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
        if (currentLevelData.bossPrefab != null)
        {
            Instantiate(currentLevelData.bossPrefab, spawnPoints[0].position, Quaternion.identity);
        }
    }

    public void OnEnemyDestroyed(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    public void SetCurrentLevel(LevelData_SO levelData)
{
    this.currentLevelData = levelData;
}


    public void EndLevel(bool victory)
    {
        Debug.Log(victory ? "LEVEL COMPLETE! YOU WIN!" : "GAME OVER! YOU LOST!");
        Time.timeScale = 0f;
        StopAllCoroutines();
    }
}
