using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Player Spawn Settings")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    // إدارة الموجات، الأعداء، إلخ...
    // public WaveManager waveManager;

    private void Start()
    {
        SpawnPlayer();
        // waveManager.StartWaves();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Player Prefab or Spawn Point not assigned!");
        }
    }
}