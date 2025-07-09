using UnityEngine;

public class KamikazeEnemy : BaseEnemy
{
    protected override void Move()
    {
        if (playerTransform != null)
        {
            // تحرك مباشرة نحو موقع اللاعب
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
        }
        else
        {
            // إذا لم يتم العثور على اللاعب، تحرك للأسفل
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }
}
