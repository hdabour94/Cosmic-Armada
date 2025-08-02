using UnityEngine;

public class ChaseMovement : IMovementBehavior
{
    private float speed;
    private Transform target;
    
    public ChaseMovement(float speed, Transform target)
    {
        this.speed = speed;
        this.target = target;
    }
    
    public void Move(Transform transform)
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }
}