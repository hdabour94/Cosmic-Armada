using UnityEngine;

[CreateAssetMenu(fileName = "StoreItem", menuName = "Store/Item", order = 0)]
public class StoreItem_SO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
    public StoreItemType itemType; // Enum: Spaceship, BossCard, PowerUp, etc.
    public GameObject prefab; // لمركبات أو عناصر تُضاف

    public CharacterStats_SO status;
}

public enum StoreItemType
{
    Spaceship,
    BossCard,
    PowerUp,
    Resource
}