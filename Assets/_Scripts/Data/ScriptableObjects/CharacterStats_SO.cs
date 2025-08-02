using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "GameData/Character Stats")]
public class CharacterStats_SO : ScriptableObject
{
    public int maxHP = 100;
    public int strength = 10;
    public int shield = 5;
    public float speed = 5f;
    public float fireRate = 0.5f;
    public int xpReward = 10;
    public int coinReward = 5;
}