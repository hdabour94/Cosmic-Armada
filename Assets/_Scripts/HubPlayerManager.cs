// HubPlayerManager.cs
using UnityEngine;

public class HubPlayerManager : MonoBehaviour
{
    [SerializeField] private Transform playerSpawnPoint;
    private GameObject currentPlayerInstance;

    void Start()
    {
        // عند بدء المشهد، قم بإنشاء المركبة الحالية للاعب
        string currentShipID = GameSessionManager.Instance.CurrentShipID;
        ShipData_SO currentShipData = GameSessionManager.Instance.GetShipDataByID(currentShipID);
        
        if (currentShipData != null)
        {
            SpawnShip(currentShipData);
        }
        else
        {
            Debug.LogError("Could not find current ship data to spawn in Hub!");
        }
    }

    public void SwapPlayerShip(ShipData_SO newShipData)
    {
        // 1. دمر المركبة القديمة إذا كانت موجودة
        if (currentPlayerInstance != null)
        {
            Destroy(currentPlayerInstance);
        }

        // 2. أنشئ المركبة الجديدة
        SpawnShip(newShipData);
    }

    private void SpawnShip(ShipData_SO shipData)
    {
        if (shipData.prefab != null && playerSpawnPoint != null)
        {
            currentPlayerInstance = Instantiate(shipData.prefab, playerSpawnPoint.position, Quaternion.identity);

            // --- مهم: تحديث كاميرا Cinemachine ---
            var cinemachineCamera = FindObjectOfType<Unity.Cinemachine.CinemachineVirtualCamera>();
            if (cinemachineCamera != null)
            {
                cinemachineCamera.Follow = currentPlayerInstance.transform;
            }
        }
    }
}