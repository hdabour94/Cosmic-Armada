using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float speed;
    private Rigidbody2D rb;

    public void Initialize(int dmg, float spd)
    {
        damage = dmg;
        speed = spd;
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        StatsManager stats = other.GetComponent<StatsManager>();
        if (stats != null)
        {
            stats.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}