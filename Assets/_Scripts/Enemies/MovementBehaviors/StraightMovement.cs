using UnityEngine;

public class StraightMovement : IMovementBehavior
{
    private float speed;
    
    public StraightMovement(float speed) => this.speed = speed;
    
    public void Move(Transform transform) => 
        transform.Translate(Vector2.down * speed * Time.deltaTime);
}