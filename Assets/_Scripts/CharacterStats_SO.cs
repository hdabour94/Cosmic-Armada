using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "Stats/Character Stats")]
public class CharacterStats_SO : ScriptableObject
{
    [Header("Base Stats")]
    public int maxHP = 100;
    public int strength = 10;
    public int shield = 5;

    public int xpReward = 10;
    public int coinReward = 5;


    public float speed;
    public float fireRate;

}