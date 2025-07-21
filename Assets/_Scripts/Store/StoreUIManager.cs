using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StoreUIManager : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject itemButtonPrefab;
    public Text coinsText;

    public List<StoreItem_SO> storeItems;

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        coinsText.text = GameSessionManager.Instance.coins.ToString();

        if (coinsText == null)
        Debug.LogError("coinsText not assigned!");
    if (itemsParent == null)
        Debug.LogError("itemsParent not assigned!");
    if (itemButtonPrefab == null)
        Debug.LogError("itemButtonPrefab not assigned!");
    if (storeItems == null)
        Debug.LogError("storeItems not assigned!");
    if (GameSessionManager.Instance == null)
        Debug.LogError("GameSessionManager.Instance is null!");
        
        foreach (Transform child in itemsParent)
            Destroy(child.gameObject);

        foreach (var item in storeItems)
        {
            var btn = Instantiate(itemButtonPrefab, itemsParent);
            btn.GetComponentInChildren<Text>().text = $"{item.itemName} - {item.price}C";
            btn.GetComponentInChildren<Image>().sprite = item.icon;
            btn.GetComponent<Button>().onClick.AddListener(() => AttemptPurchase(item));
        }
    }

    void AttemptPurchase(StoreItem_SO item)
    {
        if (GameSessionManager.Instance.coins >= item.price)
        {
            GameSessionManager.Instance.AddCoins(-item.price);
            // أضف للإنفنتوري بحسب النوع
            // TODO: InventoryManager.Add(item);
            Debug.Log($"تم شراء {item.itemName}");
            UpdateUI();
        }
        else
        {
            Debug.Log("الرصيد غير كافٍ!");
        }
    }
}