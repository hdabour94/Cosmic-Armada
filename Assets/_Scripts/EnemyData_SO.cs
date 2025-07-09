using UnityEngine;

public enum EnemyMovementType { StraightDown, SinWave, Chaser }

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemies/Enemy Data")]
public class EnemyData_SO : ScriptableObject
{
    public string enemyName;
    public Sprite enemySprite;
    public CharacterStats_SO stats; // اربطه ببيانات الإحصائيات التي أنشأناها
    public EnemyMovementType movementType;
    public float speed = 3f;

    [Header("Optional")]
    public GameObject projectilePrefab; // للعدو الذي يطلق النار
    public float fireRate = 2f;
}