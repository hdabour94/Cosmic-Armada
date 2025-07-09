using UnityEngine;
using UnityEngine.Events;

public class StatsManager : MonoBehaviour
{
    // اجعل baseStats خاصًا، لكن سنقوم بتعيينه فقط عبر Initialize
    private CharacterStats_SO baseStats;

    // الخصائص العامة للقراءة فقط
    public int CurrentHP { get; private set; }
    public CharacterStats_SO BaseStats => baseStats; // طريقة آمنة للوصول إلى بيانات الإحصائيات من الخارج

    [Header("Entity Type")]
    [Tooltip("ضع علامة صح لهذا على اللاعب فقط للتمييز في السلوكيات")]
    public bool isPlayer = false;

    // UnityEvent لإدارة ما يحدث عند الموت
    public UnityEvent OnDie;

    // Awake تكون فارغة الآن لأن التهيئة ستتم في Initialize
    private void Awake()
    {
        // لا تضع أي كود يعتمد على baseStats هنا
    }

    // هذه هي الدالة الرئيسية لتهيئة أي كائن لديه إحصائيات
    public void Initialize(CharacterStats_SO statsToAssign)
    {
        // 1. تحقق من أن الـ stats التي تم تمريرها ليست null لتجنب الأخطاء
        if (statsToAssign == null)
        {
            Debug.LogError("StatsManager on " + gameObject.name + " received null stats!");
            return;
        }

        // 2. قم بتعيين الـ baseStats
        this.baseStats = statsToAssign;

        // 3. الآن بعد أن تم تعيين baseStats، قم بتهيئة القيم
        CurrentHP = baseStats.maxHP;

        // 4. قم بتحديث الواجهة إذا كان هذا هو اللاعب
        if (isPlayer)
        {
            UIManager.Instance?.UpdateHealthUI(CurrentHP, baseStats.maxHP);
        }

        // 5. ربط حدث الموت بمنح المكافآت (فقط إذا لم يكن لاعبًا)
        if (!isPlayer)
        {
            // تأكد من أن GameManager.Instance موجود قبل إضافة المستمعين
            if (GameManager.Instance != null)
            {
                // نستخدم قيم المكافآت من الـ ScriptableObject (أفضل للمستقبل)
                // افترض أنك أضفت هذه المتغيرات إلى CharacterStats_SO
                // OnDie.AddListener(() => GameManager.Instance.AddXP(baseStats.xpReward));
                // OnDie.AddListener(() => GameManager.Instance.AddCoins(baseStats.coinReward));

                // للوقت الحالي، سنستخدم قيماً ثابتة كما في كودك
                OnDie.AddListener(() => GameManager.Instance.AddXP(50));
                OnDie.AddListener(() => GameManager.Instance.AddCoins(10));
            }
            else
            {
                Debug.LogError("GameManager.Instance is not set when trying to initialize enemy rewards!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // لا تفعل شيئًا إذا كان ميتًا بالفعل
        if (CurrentHP <= 0) return;

        CurrentHP -= damage;

        if (isPlayer)
        {
            // استخدم "?." (null-conditional operator) كإجراء احترازي
            UIManager.Instance?.UpdateHealthUI(CurrentHP, baseStats.maxHP);
        }

        if (CurrentHP <= 0)
        {
            CurrentHP = 0; // لمنع القيم السالبة في الواجهة
            Die();
        }
    }

    public void Die()
    {
        // استدعاء جميع الدوال المرتبطة بحدث الموت
        OnDie?.Invoke();

        // إزالة جميع المستمعين لمنع استدعائهم مرة أخرى عن طريق الخطأ
        OnDie.RemoveAllListeners();

        if (isPlayer)
        {
            // منطق موت اللاعب (مثل عرض شاشة "Game Over" وتعطيل الكائن)
            // GameManager.Instance.EndLevel(false);
            gameObject.SetActive(false); // هذا جيد للمرحلة الحالية
        }
        else
        {
            // منطق موت العدو
            GameManager.Instance?.OnEnemyDestroyed(this.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    // هذا الكود سيعمل فقط على الكائن الذي يحمل StatsManager (اللاعب والأعداء)

    // --- الضرر على اللاعب ---
    if (isPlayer) // هل هذا هو اللاعب؟
    {
        // 1. الضرر من الاصطدام الجسدي بالعدو
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Player collided with an enemy!");
            // أخذ ضرر من إحصائيات العدو الذي اصطدم بنا
            StatsManager enemyStats = other.GetComponent<StatsManager>();
            if (enemyStats != null)
            {
                // يمكن أن يكون الضرر هو قوة العدو أو قيمة ثابتة
                int collisionDamage = enemyStats.BaseStats.strength; // مثال: ضرر الاصطدام يساوي قوة العدو
                TakeDamage(collisionDamage);

                // اختياري: تدمير العدو عند الاصطدام باللاعب (سلوك انتحاري)
                enemyStats.TakeDamage(9999); // تدميره فورًا
            }
        }

        // 2. الضرر من رصاص العدو
        if (other.CompareTag("EnemyProjectile"))
        {
            Debug.Log("Player was hit by an enemy projectile!");
            // أخذ ضرر من الرصاصة
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.damage);
            }
            // تدمير رصاصة العدو بعد الاصطدام
            Destroy(other.gameObject);
        }
    }

    // --- الضرر على العدو ---
    // (هذا الجزء موجود بالفعل في EnemyAI.cs و BossAI.cs، لكن وضعه هنا يجعله مركزيًا)
    // إذا أردت جعل StatsManager هو المسؤول الوحيد عن تلقي الضرر، يمكنك نقل المنطق إلى هنا
    if (!isPlayer) // هل هذا عدو؟
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.damage);
            }
            Destroy(other.gameObject);
        }
    }
}
}