using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Levels/Level Data")]
public class LevelData_SO : ScriptableObject
{
    // المتغيرات التي أضفناها سابقًا
    public string levelName;

    // المتغيرات الجديدة لحل الأخطاء الحالية
    public int levelIndex;          // <<<--- السطر الجديد الأول
    public string sceneToLoad;      // <<<--- السطر الجديد الثاني

    [Header("Level Content")] // تنظيم بسيط للـ Inspector
    public WaveData_SO[] waves;
    public GameObject bossPrefab;
    public float timeBeforeBoss = 5f;

    public BossData_SO bossData;
    
}