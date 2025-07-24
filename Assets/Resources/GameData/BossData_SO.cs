using UnityEngine;

[CreateAssetMenu(fileName = "New Boss", menuName = "Game Data/Boss")]
public class BossData_SO : ScriptableObject
{
    public string bossID;              // معرّف فريد للزعيم
    public string bossName;            // الاسم الظاهر
    public Sprite icon;                // أيقونة يمكن عرضها عند الاقتراب من الزعيم أو في شاشة خاصة
    public GameObject prefab;          // الـ prefab الفعلي الذي يتم استدعاؤه عند القتال

    [Header("Stats")]
    public int maxHP = 500;
    public int strength = 25;
    public float fireRate = 1.2f;
    public float moveSpeed = 2f;

    [Header("Rewards")]
    public int xpReward = 100;
    public int currencyReward = 50;
    // يمكنك لاحقًا ربطه بـ LootTable أو DropItems إن رغبت
}
