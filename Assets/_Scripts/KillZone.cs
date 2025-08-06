using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy(other.gameObject); // <<<--- احذف هذا السطر فورًا

        if (other.CompareTag("Enemy"))
        {
            StatsManager enemyStats = other.GetComponent<StatsManager>();
            if (enemyStats != null)
            {
                enemyStats.OnDie.RemoveAllListeners(); // لا تمنح مكافآت
                enemyStats.Die();
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("EnemyProjectile") || other.CompareTag("PlayerProjectile"))
        {
            Destroy(other.gameObject);
        }
    }
}