using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(other.gameObject);
        // تحقق مما إذا كان الكائن الذي دخل هو عدو
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);

            Debug.Log(other.name + " has left the screen and is being destroyed.");

            // نحتاج إلى التأكد من أن GameManager يعرف أن هذا العدو قد "مات"
            // حتى لو لم يتم قتله بواسطة اللاعب (لكي لا تتعطل الموجة).

            // الطريقة الأفضل هي استدعاء دالة الموت في العدو نفسه
            StatsManager enemyStats = other.GetComponent<StatsManager>();
            if (enemyStats != null)
            {
                // استدعاء Die() مباشرة سيضمن إزالته من قائمة GameManager
                // وتنفيذ أي منطق آخر متعلق بالموت (لكن بدون منح مكافآت).

                // يمكنك إزالة المستمعين الذين يمنحون المكافآت قبل استدعاء الموت
                enemyStats.OnDie.RemoveAllListeners();
                enemyStats.Die(); // استدعاء دالة الموت العامة.
            }
            else
            {
                // كحل بديل إذا لم يكن هناك StatsManager لسبب ما
                Destroy(other.gameObject);
            }
        }
        // اختياري: يمكنك أيضًا تدمير رصاص الأعداء الذي يخرج من الشاشة
        else if (other.CompareTag("EnemyProjectile"))
        {
            Destroy(other.gameObject);
        }
    }
}