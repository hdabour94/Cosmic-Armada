using UnityEngine;

[CreateAssetMenu(fileName = "New Boss Data", menuName = "GameData/Boss Data")]
public class BossData_SO : ScriptableObject
{
    public string bossName;
    public GameObject prefab;
    public CharacterStats_SO characterStats; // هذا الحقل كان ناقصًا
    public GameObject projectilePrefab;
    public BossPattern[] patterns;
    
    [Header("Other Settings")]
    public float timeBetweenPatterns = 3f;
    public int xpReward = 100;
    public int coinReward = 50;
}