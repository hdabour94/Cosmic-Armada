using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private LevelData_SO currentLevelData;

    [Header("Player Configuration")]
    private ShipData_SO selectedShipData;
    [SerializeField] private Transform playerSpawnPoint;

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
    selectedShipData = GameDataHolder.Instance.SelectedShip;

    if (selectedShipData == null)
    {
        Debug.LogError("No ship selected! Falling back to default prefab.");
        return;
    }

    GameObject playerGO = Instantiate(selectedShipData.prefab, playerSpawnPoint.position, Quaternion.identity);

    StatsManager sm = playerGO.GetComponent<StatsManager>();
    if (sm != null)
    {
        sm.BaseStats.maxHP = selectedShipData.maxHP;
        sm.BaseStats.strength = selectedShipData.strength;
        sm.BaseStats.speed = selectedShipData.speed;
        sm.BaseStats.fireRate = selectedShipData.fireRate;
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

        if (currentLevelData.bossData != null && currentLevelData.bossData.prefab != null)
        {
            GameObject bossGO = Instantiate(currentLevelData.bossData.prefab, spawnPoints[0].position, Quaternion.identity);
            StatsManager sm = bossGO.GetComponent<StatsManager>();
            if (sm != null)
            {
                sm.BaseStats.maxHP = currentLevelData.bossData.maxHP;
                sm.BaseStats.strength = currentLevelData.bossData.strength;
                sm.BaseStats.fireRate = currentLevelData.bossData.fireRate;
            }
        }
        else
        {
            Debug.LogWarning("No boss data assigned in currentLevelData!");
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
