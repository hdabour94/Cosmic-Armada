using UnityEngine;

public class GameDataHolder : MonoBehaviour
{
    public static GameDataHolder Instance { get; private set; }
    
    public LevelData_SO SelectedLevel { get; private set; }

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

    public void SetSelectedLevel(LevelData_SO levelData)
    {
        SelectedLevel = levelData;
    }
}