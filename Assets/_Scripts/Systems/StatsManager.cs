using UnityEngine;
using UnityEngine.Events;

public class StatsManager : MonoBehaviour
{
    public CharacterStats_SO BaseStats { get; private set; }
    public int CurrentHP { get; private set; }
    public bool IsPlayer = false;
    
    public UnityEvent OnDie;
    private RewardSystem rewardSystem;

        private EnemyUIController uiController;

  //  [System.Obsolete]
    private void Awake()
    {
        rewardSystem = FindObjectOfType<RewardSystem>();
        if (!rewardSystem) rewardSystem = gameObject.AddComponent<RewardSystem>();
    }

    public void Initialize(CharacterStats_SO stats)
    {
        BaseStats = stats;
        CurrentHP = BaseStats.maxHP;

        if (IsPlayer)
        {
            UIManager.Instance?.UpdateHealthUI(CurrentHP, BaseStats.maxHP);

            if (uiController != null)
            {
                // قم بتهيئته وأعطه الهدف الذي يجب أن يتبعه (وهو هذا العدو)
                uiController.Initialize(this.transform);

                // قم بتحديث الواجهة بالقيم الأولية
                uiController.UpdateHealth(CurrentHP, BaseStats.maxHP);

                // افترض أن لديك مستوى في CharacterStats_SO
                // إذا لم يكن كذلك، يمكنك تجاهل هذا السطر
                // uiController.UpdateLevel(BaseStats.level); 
            }
        }
         else
        {
            uiController = GetComponentInChildren<EnemyUIController>();
            if (uiController != null)
            {
                // قم بتهيئته وأعطه الهدف الذي يجب أن يتبعه (وهو هذا العدو)
                uiController.Initialize(this.transform);

                // قم بتحديث الواجهة بالقيم الأولية
                uiController.UpdateHealth(CurrentHP, BaseStats.maxHP);
                
                // يمكنك إضافة منطق المستوى هنا إذا أردت
                // uiController.UpdateLevel(BaseStats.level); 
            }
            // --- ربط نظام المكافآت بحدث الموت ---
            OnDie.AddListener(GrantRewards);
            // ------------------------------------
        }
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHP <= 0) return;
        
        CurrentHP = Mathf.Max(0, CurrentHP - damage);
        
        if (IsPlayer)
        {
            UIManager.Instance?.UpdateHealthUI(CurrentHP, BaseStats.maxHP);
        }else // إذا كان عدوًا
        {
            // قم بتحديث شريط الصحة الخاص به
            uiController?.UpdateHealth(CurrentHP, BaseStats.maxHP);
        }

        if (CurrentHP <= 0) Die();
    }

    public void Die()
    {
        OnDie?.Invoke();

        if (IsPlayer)
        {
            LevelManager.Instance?.EndLevel(false);
            gameObject.SetActive(false);
        }
        else // هذا عدو
        {
            // --- الجزء الرئيسي للحل ---
            // 1. أخبر المدير بأن هذا العدو قد تم تدميره
            LevelManager.Instance?.OnEnemyDestroyed(this.gameObject);

            // 2. دمر الكائن
            Destroy(gameObject);
        }
    }
private void GrantRewards()
    {
        if (rewardSystem != null && BaseStats != null)
        {
            rewardSystem.HandleEnemyDeath(BaseStats);
        }
        // أزل المستمع لمنع استدعائه مرة أخرى عن طريق الخطأ
        OnDie.RemoveListener(GrantRewards);
    }
}