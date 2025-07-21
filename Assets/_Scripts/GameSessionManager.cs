using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    public int playerLevel = 1;
    public int playerXP = 0;
    public int xpToNextLevel = 100;
    public int coins = 0;

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
        playerXP += xpAmount;
        UIManager.Instance?.UpdateXPUI(playerXP, xpToNextLevel);
        if (playerXP >= xpToNextLevel) LevelUp();
    }

    private void LevelUp()
    {
        playerLevel++;
        playerXP -= xpToNextLevel;
        xpToNextLevel = (int)(xpToNextLevel * 1.5f);
        UIManager.Instance?.UpdateLevelText(playerLevel);
        UIManager.Instance?.UpdateXPUI(playerXP, xpToNextLevel);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UIManager.Instance?.UpdateCoinText(coins);
    }

    public void SaveGame()
    {
        PlayerData data = new PlayerData
        {
            playerLevel = playerLevel,
            playerXP = playerXP,
            xpToNextLevel = xpToNextLevel,
            coins = coins
        };
        SaveSystem.SavePlayer(data);
    }

    public void LoadGame()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        playerLevel = data.playerLevel;
        playerXP = data.playerXP;
        xpToNextLevel = data.xpToNextLevel;
        coins = data.coins;

        UIManager.Instance?.UpdateCoinText(coins);
        UIManager.Instance?.UpdateLevelText(playerLevel);
        UIManager.Instance?.UpdateXPUI(playerXP, xpToNextLevel);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
