using System.Collections;
using UnityEngine;

public class FormationEnemyAI : MonoBehaviour
{
    private enum State { InFormation, Attacking, Returning }
    private State currentState;

    [Header("Settings")]
    [SerializeField] private float attackDuration = 2f; // مدة ملاحقة اللاعب
    [SerializeField] private float returnSpeed = 8f; // سرعة العودة للتشكيل
    [SerializeField] private float attackSpeed = 6f; // سرعة الهجوم

    private Transform formationPoint;
    private Transform playerTransform;
    private StatsManager stats;
    private FormationController formationController;

    void Awake()
    {
        stats = GetComponent<StatsManager>();
        stats.OnDie.AddListener(OnDeath);
    }

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentState = State.InFormation;
    }

    public void AssignToPoint(Transform point)
    {
        this.formationPoint = point;
        this.formationController = point.GetComponentInParent<FormationController>();
    }

    void Update()
    {
        if (formationPoint == null) return;

        switch (currentState)
        {
            case State.InFormation:
                // ببساطة، اجعل موقعك هو موقع نقطة التشكيل
                transform.position = formationPoint.position;
                transform.rotation = formationPoint.rotation;
                break;

            case State.Attacking:
                // لاحق اللاعب
                if (playerTransform != null)
                {
                    Vector3 direction = (playerTransform.position - transform.position).normalized;
                    transform.Translate(direction * attackSpeed * Time.deltaTime, Space.World);
                }
                break;

            case State.Returning:
                // عد بسرعة إلى نقطة التشكيل
                transform.position = Vector2.MoveTowards(transform.position, formationPoint.position, returnSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, formationPoint.position) < 0.1f)
                {
                    currentState = State.InFormation; // وصلت إلى مكانك
                }
                break;
        }
    }

    public void StartAttack()
    {
        if (currentState == State.InFormation)
        {
            StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
        currentState = State.Attacking;
        yield return new WaitForSeconds(attackDuration);
        currentState = State.Returning;
    }

    private void OnDeath()
    {
        // أخبر قائد السرب بأنك مت
        if (formationController != null)
        {
            formationController.RemoveEnemy(this);
        }
    }
    
    // يجب إضافة OnTriggerEnter2D للتعامل مع رصاص اللاعب
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            stats.TakeDamage(other.GetComponent<Projectile>().damage);
            Destroy(other.gameObject);
        }
    }
}
