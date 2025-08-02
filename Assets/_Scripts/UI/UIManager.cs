using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Player HUD")]
    public Slider healthSlider;
    public Slider xpSlider;
    public TMP_Text levelText;
    public TMP_Text coinText;

    public TMP_Text NotificatiomText;

    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject levelCompleteMenu;

    void Awake() => Instance = this;

    public void UpdateHealthUI(int current, int max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }

    public void UpdateXPUI(int current, int required)
    {
        xpSlider.maxValue = required;
        xpSlider.value = current;
    }

    public void UpdateLevelText(int level) => levelText.text = $"Level {level}";
    
    public void UpdateCoinText(int amount) => coinText.text = amount.ToString();

    public void ShowLevelComplete(bool victory)
    {
        levelCompleteMenu.SetActive(true);
        // إضافة منطق إضافي للفوز/الخسارة
    }

    public void ShowNotification(string message)
{
        NotificatiomText.text = message;
}
}