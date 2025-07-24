using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailsUI : MonoBehaviour
{
    public GameObject panelRoot;
    public Image iconImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI priceText;
    public Button buyButton;
    public Button closeButton;

    private StoreItem_SO currentItem;

    void Start()
    {
        closeButton.onClick.AddListener(() => panelRoot.SetActive(false));
    }

    public void ShowItem(StoreItem_SO item, System.Action<StoreItem_SO> onPurchase)
    {
        currentItem = item;

        iconImage.sprite = item.icon;
        itemNameText.text = item.itemName;
        priceText.text = $"{item.price} C";

        // عرض الإحصائيات لو كانت المركبة مرتبطة بـ CharacterStats
        if (item.itemType == StoreItemType.Spaceship && item.prefab != null)
        {
           // var stats = item.prefab.GetComponent<StatsManager>()?.BaseStats;
            var stats = item.prefab.GetComponent<StatsManager>()?.BaseStats;
            if (stats != null)
            {
                statsText.text = $"HP: {stats.maxHP}\nPower: {stats.strength}";
            }
            else
            {
                statsText.text = "No stats found.";
            }
        }
        else
        {
            statsText.text = "لا توجد خصائص.";
        }

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => {
            onPurchase?.Invoke(currentItem);
            panelRoot.SetActive(false);
        });

        panelRoot.SetActive(true);
    }
}
