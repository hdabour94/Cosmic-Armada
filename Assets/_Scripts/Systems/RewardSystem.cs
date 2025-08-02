using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public void HandleEnemyDeath(CharacterStats_SO enemyStats)
    {
        if (enemyStats.xpReward > 0)
            GameSessionManager.Instance.AddXP(enemyStats.xpReward);
        
        if (enemyStats.coinReward > 0)
            GameSessionManager.Instance.AddCoins(enemyStats.coinReward);
    }
}