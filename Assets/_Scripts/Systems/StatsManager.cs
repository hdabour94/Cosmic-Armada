using UnityEngine;
using UnityEngine.Events;

public class StatsManager : MonoBehaviour
{
    public CharacterStats_SO BaseStats { get; private set; }
    public int CurrentHP { get; private set; }
    public bool IsPlayer = false;
    
    public UnityEvent OnDie;
    private RewardSystem rewardSystem;

    [System.Obsolete]
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
        }
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHP <= 0) return;
        
        CurrentHP = Mathf.Max(0, CurrentHP - damage);
        
        if (IsPlayer)
        {
            UIManager.Instance?.UpdateHealthUI(CurrentHP, BaseStats.maxHP);
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
    else
    {
        Destroy(gameObject, 0.1f);
    }
}
}