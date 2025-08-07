using UnityEngine;
using System.Collections.Generic;
using System.Linq; // سنحتاج هذا لإحدى التحسينات

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    // --- هذه هي المتغيرات "الحية" التي تستخدمها اللعبة ---
    public int PlayerLevel { get; private set; } = 1;
    public int PlayerXP { get; private set; } = 0;
    public int XpToNextLevel { get; private set; } = 100;
    public int Coins { get; private set; } = 0;

    public string CurrentShipID { get; private set; }
    public List<string> UnlockedShipIDs { get; private set; } = new List<string>();

    // ----------------------------------------------------

    [Header("Game Configuration")]
    [Tooltip("اسحب كل ملفات ShipData_SO المتاحة في اللعبة هنا")]
    [SerializeField] private List<ShipData_SO> allAvailableShips;
    internal readonly IEnumerable<object> unlockedShipIDs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddXP(int xpAmount)
    {
        PlayerXP += xpAmount;
        UIManager.Instance?.UpdateXPUI(PlayerXP, XpToNextLevel);
        if (PlayerXP >= XpToNextLevel) LevelUp();
    }

    private void LevelUp()
    {
        PlayerLevel++;
        PlayerXP -= XpToNextLevel;
        XpToNextLevel = (int)(XpToNextLevel * 1.5f);
        UIManager.Instance?.UpdateLevelText(PlayerLevel);
        UIManager.Instance?.UpdateXPUI(PlayerXP, XpToNextLevel);
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        UIManager.Instance?.UpdateCoinText(Coins);
    }

    public void UnlockShip(string shipID)
    {
        if (!UnlockedShipIDs.Contains(shipID))
        {
            UnlockedShipIDs.Add(shipID);
            Debug.Log($"Ship Unlocked: {shipID}");
        }
    }

    public void SetCurrentShip(string shipID)
    {
        if (UnlockedShipIDs.Contains(shipID))
        {
            CurrentShipID = shipID;
            GameDataHolder.Instance.SetSelectedShip(GetShipDataByID(shipID));
            Debug.Log($"Current ship set to: {shipID}");
        }
    }

    public ShipData_SO GetShipDataByID(string shipID)
    {
        // استخدام FirstOrDefault أكثر أمانًا من Find
        return allAvailableShips.FirstOrDefault(ship => ship.name == shipID);
    }

    // --- دوال الحفظ والتحميل المحدثة ---
    public void SaveGame()
    {
        PlayerData data = new PlayerData
        {
            playerLevel = this.PlayerLevel,
            playerXP = this.PlayerXP,
            xpToNextLevel = this.XpToNextLevel,
            coins = this.Coins,
            currentShipID = this.CurrentShipID,
            unlockedShipIDs = this.UnlockedShipIDs
        };
        SaveSystem.SavePlayer(data);
    }

    public void LoadGame()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        
        this.PlayerLevel = data.playerLevel > 0 ? data.playerLevel : 1;
        this.PlayerXP = data.playerXP;
        this.XpToNextLevel = data.xpToNextLevel > 0 ? data.xpToNextLevel : 100;
        this.Coins = data.coins;

        this.UnlockedShipIDs = data.unlockedShipIDs ?? new List<string>();
        
        // تأكد من أن اللاعب يبدأ بمركبة افتراضية على الأقل
        if (this.UnlockedShipIDs.Count == 0 && allAvailableShips.Count > 0)
        {
            UnlockShip(allAvailableShips[0].name); // افترض أن أول مركبة هي الافتراضية
        }

        this.CurrentShipID = data.currentShipID;
        if (string.IsNullOrEmpty(this.CurrentShipID) || !this.UnlockedShipIDs.Contains(this.CurrentShipID))
        {
            this.CurrentShipID = this.UnlockedShipIDs.Count > 0 ? this.UnlockedShipIDs[0] : "";
        }
        
        // قم بتعيين المركبة الحالية في GameDataHolder عند بدء الجلسة
        if (!string.IsNullOrEmpty(this.CurrentShipID))
        {
            SetCurrentShip(this.CurrentShipID);
        }

        // قم بتحديث الواجهة (إذا كانت موجودة)
        // UIManager.Instance?.UpdateAllUI(...);
    }

    private void OnApplicationQuit() => SaveGame();
}