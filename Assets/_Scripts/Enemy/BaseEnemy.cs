using UnityEngine;

// [RequireComponent(typeof(StatsManager))] // يضمن وجود StatsManager دائمًا
public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Base Enemy Settings")]
    public float speed = 3f;
    public int xpOnDeath = 10;
    public int coinsOnDeath = 5;

    protected StatsManager stats;
    protected Transform playerTransform; // لتتبع اللاعب

    protected virtual void Awake()
    {
        stats = GetComponent<StatsManager>();
        // ربط حدث الموت بالدالة المحلية
        if (stats != null)
        {
            stats.OnDie.AddListener(HandleDeath);
        }
    }

    protected virtual void Start()
    {
        // ابحث عن اللاعب مرة واحدة فقط لتجنب البحث المستمر
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // تأكد من أن مركبة اللاعب لديها Tag "Player"
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    // هذه الدالة ستكون مختلفة لكل نوع من الأعداء
    protected abstract void Move();

    void Update()
    {
        Move(); // استدعاء دالة الحركة في كل إطار
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            int damage = other.GetComponent<Projectile>().damage;
            stats.TakeDamage(damage);
            Destroy(other.gameObject);
        }
    }

    private void HandleDeath()
    {
        // منح الخبرة والعملات عند الموت
      //  GameManager.Instance.AddRewards(xpReward, coinReward);
       // GameManager.Instance.AddXP(xpOnDeath);
       //  GameManager.Instance.AddCoins(coinsOnDeath); // عندما تضيف نظام العملات
        
        // يمكنك إضافة تأثير انفجار هنا قبل التدمير
        // Destroy(gameObject); // يتم التدمير الآن من خلال StatsManager
    }
}