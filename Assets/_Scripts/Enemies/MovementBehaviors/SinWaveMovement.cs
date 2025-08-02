using UnityEngine;

public class SinWaveMovement : IMovementBehavior
{
    private float speed;
    private float frequency;
    private float amplitude;
    private Vector3 startPos;
    
    public SinWaveMovement(float speed, float freq, float amp)
    {
        this.speed = speed;
        frequency = freq;
        amplitude = amp;
    }
    
    public void Move(Transform transform)
    {
        if (startPos == Vector3.zero) startPos = transform.position;
        
        float x = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + new Vector3(x, 0, 0) + 
            (Vector3.down * speed * Time.time);
    }
}