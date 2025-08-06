using System.Collections;
using UnityEngine;

[RequireComponent(typeof(StatsManager))]
[RequireComponent(typeof(SpriteRenderer))]
public class FormationEnemyAI : MonoBehaviour
{
    // الحالات التي يمكن أن يكون فيها العدو
    private enum State { InFormation, Attacking, Returning, Disabled }
    private State currentState;

    [Header("Behavior Settings")]
    [SerializeField] private float attackDuration = 2.5f; // مدة ملاحقة اللاعب
    [SerializeField] private float returnSpeed = 8f; // سرعة العودة للتشكيل
    [SerializeField] private float attackSpeed = 6f; // سرعة الهجوم أثناء الملاحقة

    // مراجع لمكونات وبيانات أساسية
    private EnemyData_SO enemyData;
    private Transform formationPoint;
    private FormationController formationController;
    private Transform playerTransform;

    // مكونات يتم جلبها تلقائيًا (Caching)
    private StatsManager stats;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // جلب المكونات من نفس الكائن لتجنب GetComponent المتكرر
        stats = GetComponent<StatsManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ربط حدث الموت بالدالة المحلية
        if (stats != null)
        {
            stats.OnDie.AddListener(OnDeath);
        }
    }

    // هذه هي الدالة الرئيسية التي تعطي العدو هويته
    public void Initialize(EnemyData_SO data)
    {
        this.enemyData = data;
        if (this.enemyData == null)
        {
            Debug.LogError("EnemyData_SO is null for " + gameObject.name + ". Disabling AI.");
            currentState = State.Disabled;
            return;
        }

        // 1. تطبيق البيانات المرئية
        spriteRenderer.sprite = enemyData.enemySprite;

        // 2. تهيئة نظام الإحصائيات بالبيانات الصحيحة
        stats.Initialize(enemyData.stats);

        // 3. البحث عن اللاعب (هدف الهجوم)
        // من الأفضل جلبه من GameManager لضمان وجود النسخة الصحيحة
        if (LevelManager.Instance != null && LevelManager.Instance.CurrentPlayerInstance != null)
        {
            playerTransform = LevelManager.Instance.CurrentPlayerInstance.transform;
        }

        // 4. ضبط الحالة الأولية
        currentState = State.InFormation;
    }

    // هذه الدالة تربط العدو بنقطته في التشكيل
    public void AssignToPoint(Transform point, FormationController controller) // Corrected method name
    {
        this.formationPoint = point;
        this.formationController = controller;
    }

    void Update()
{
    if (currentState == State.Disabled) return;

    switch (currentState)
    {
        case State.InFormation:
            // لا حاجة لعمل أي شيء هنا. نظام الـ Parent/Child يقوم بالعمل.
            break;

        case State.Attacking:
            // ملاحقة اللاعب (مع التحقق من وجوده)
            if (playerTransform != null)
            {
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                transform.Translate(direction * attackSpeed * Time.deltaTime, Space.World);
            }
            else
            {
                // إذا دُمر اللاعب، عد إلى التشكيل
                currentState = State.Returning;
            }
            break;

        case State.Returning:
            // العودة بسرعة إلى نقطة التشكيل
            if (formationPoint != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, formationPoint.position, returnSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, formationPoint.position) < 0.1f)
                {
                    // تم الوصول، أعد ربط نفسك بالتشكيل
                    currentState = State.InFormation;
                    transform.SetParent(formationPoint);
                    transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                // إذا تم تدمير التشكيل لسبب ما، تصرف كعدو عادي
                transform.Translate(Vector2.down * attackSpeed * Time.deltaTime);
            }
            break;
    }
}

    // يتم استدعاؤها من FormationController لبدء الهجوم
    public void StartAttack()
    {
        if (currentState == State.InFormation)
        {
            StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
{
    if (currentState != State.InFormation) yield break;

    currentState = State.Attacking;
    transform.SetParent(null); // افصل نفسك عن التشكيل لتبدأ الهجوم بحرية
    
    yield return new WaitForSeconds(attackDuration);
    
    // بعد انتهاء مدة الهجوم، ابدأ بالعودة
    currentState = State.Returning;
}
    // دالة تُستدعى عند موت العدو (بفضل UnityEvent)
    private void OnDeath()
    {
        
        if (formationController != null)
        {
            formationController.RemoveEnemy(this);
        }
        // Also notify LevelManager/GameManager that this enemy is gone
     /*   if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnEnemyDestroyed(gameObject);
        }
        else if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnEnemyDestroyed(gameObject);
        }*/
    }
}