﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // تأكد من تثبيت TextMeshPro من Package Manager

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Player UI Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI maxHpTxt;
    [SerializeField] private TextMeshProUGUI currntHPTxt;

    [SerializeField] private TextMeshProUGUI xpToNextLevelTxt;
    [SerializeField] private TextMeshProUGUI currentXpTxt;



    [Header("Notification UI")]
    [SerializeField] private GameObject notificationPanel; // لوحة تحتوي على النص
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float notificationDisplayTime = 2f;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // تحديث الواجهة بالقيم الأولية من GameManager عند بدء المشهد
        if (GameSessionManager.Instance != null)
        {
            UpdateLevelText(GameSessionManager.Instance.playerLevel);
            UpdateXPUI(GameSessionManager.Instance.playerXP, GameSessionManager.Instance.xpToNextLevel);
            UpdateCoinText(GameSessionManager.Instance.coins);
        }
    }

    public void UpdateHealthUI(int currentHP, int maxHP)
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHP / maxHP;
            maxHpTxt.text = maxHP.ToString();
            currntHPTxt.text = currentHP.ToString();

        }
    }

    public void UpdateXPUI(int currentXP, int xpToNextLevel)
    {
        if (xpSlider != null)
        {
            xpSlider.value = (float)currentXP / xpToNextLevel;
            currentXpTxt.text = currentXP.ToString();
            xpToNextLevelTxt.text = xpToNextLevel.ToString();
        }
    }

    public void UpdateLevelText(int level)
    {
        if (levelText != null)
        {
            levelText.text = "LVL: " + level.ToString();
        }
    }

    public void UpdateCoinText(int amount)
    {
        if (coinText != null)
        {
            coinText.text = amount.ToString();
        }
    }

    public void ShowNotification(string message)
    {
        if (notificationPanel != null && notificationText != null)
        {
            StartCoroutine(NotificationRoutine(message));
        }
    }

    private IEnumerator NotificationRoutine(string message)
    {
        // أظهر اللوحة واضبط النص
        notificationPanel.SetActive(true);
        notificationText.text = message;

        // انتظر لمدة محددة
        yield return new WaitForSeconds(notificationDisplayTime);

        // أخفِ اللوحة
        notificationPanel.SetActive(false);
    }
}