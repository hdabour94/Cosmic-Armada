using UnityEngine;

[CreateAssetMenu(fileName = "New Ship", menuName = "Game Data/Ship")]
public class ShipData_SO : ScriptableObject
{
    public string shipID;              // معرّف فريد للمركبة
    public string shipName;            // اسم المركبة الظاهر
    public Sprite icon;                // أيقونة تستخدم في المتجر أو واجهة اللاعب
    public GameObject prefab;          // prefab الخاص بالمركبة

    [Header("Stats")]
    public int maxHP = 100;
    public int strength = 10;
    public float speed = 5f;
    public float fireRate = 0.5f;
    public float shield = 0f;
}
