// EnemyUIController.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUIController : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI levelText;

    private Transform targetToFollow; // العدو الذي سيتم اتباعه
    private Vector3 offset;           // المسافة فوق العدو

    // يتم استدعاؤها من العدو لتهيئته
    public void Initialize(Transform target)
    {
        this.targetToFollow = target;
        // احفظ المسافة الأولية بين الـ UI والعدو
        offset = transform.position - target.position;
    }

    // اجعل الـ UI يواجه الكاميرا دائمًا
    private void LateUpdate()
    {
        // إذا تم تدمير الهدف، دمر نفسك أيضًا
        if (targetToFollow == null)
        {
            Destroy(gameObject);
            return;
        }

        // اجعل الواجهة تتبع العدو
        transform.position = targetToFollow.position + offset;

        // اجعل الواجهة تنظر دائمًا إلى الكاميرا
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    // دالة لتحديث شريط الصحة
    public void UpdateHealth(int currentHP, int maxHP)
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHP / maxHP;
        }
    }

    // دالة لتحديث نص المستوى
    public void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Lvl {level}";
            levelText.gameObject.SetActive(level > 0); // أظهر المستوى فقط إذا كان له قيمة
        }
    }
}