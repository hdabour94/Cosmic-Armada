using System.Collections;
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

    [Header("Notifications")]
    public TMP_Text notificationText; // تأكد من تغيير الاسم ليطابق الكود
    public float notificationDisplayTime = 2.5f; // المدة التي سيظهر فيها الإشعار بالثواني

    private Coroutine currentNotificationCoroutine; // لتخزين الكوروتين الحالي

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
        if (notificationText == null) return;

        // إذا كان هناك إشعار قديم يعمل، أوقفه أولاً
        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine);
        }

        // ابدأ كوروتين جديد لإظهار الإشعار الحالي
        currentNotificationCoroutine = StartCoroutine(NotificationRoutine(message));
    }
    private IEnumerator NotificationRoutine(string message)
    {
        // 1. أظهر النص وفعّل الكائن
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);

        // 2. انتظر للمدة المحددة
        yield return new WaitForSeconds(notificationDisplayTime);

        // 3. أخفِ النص وقم بإلغاء تفعيل الكائن
        notificationText.text = "";
        notificationText.gameObject.SetActive(false);
        
        // أفرغ المرجع بعد انتهاء الكوروتين
        currentNotificationCoroutine = null;
    }
    
}