using UnityEngine;

public class GameDataHolder : MonoBehaviour
{
    public static GameDataHolder Instance { get; private set; }

    public ShipData_SO SelectedShip { get; private set; }
    public LevelData_SO SelectedLevel { get; private set; } // 🟢 تم إضافته الآن

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

    public void SetSelectedShip(ShipData_SO ship)
    {
        SelectedShip = ship;
    }

    public void SetSelectedLevel(LevelData_SO level) // ✅ دالة جديدة
    {
        SelectedLevel = level;
    }
}
