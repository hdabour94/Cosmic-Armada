using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Player Spawn Settings")]
    public Transform playerSpawnPoint;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var shipData = GameDataHolder.Instance.SelectedShip;

        if (shipData == null)
        {
            Debug.LogError("No ship selected! تأكد من أنك اخترت مركبة قبل الدخول للمرحلة.");
            return;
        }

        GameObject playerGO = Instantiate(shipData.prefab, playerSpawnPoint.position, Quaternion.identity);

        StatsManager sm = playerGO.GetComponent<StatsManager>();
        if (sm != null)
        {
            sm.BaseStats.maxHP = shipData.maxHP;
            sm.BaseStats.strength = shipData.strength;
            sm.BaseStats.speed = shipData.speed;
            sm.BaseStats.fireRate = shipData.fireRate;
        }
        else
        {
            Debug.LogWarning("StatsManager غير موجود على prefab اللاعب.");
        }
    }
}
