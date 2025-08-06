using UnityEngine;

public class SinWaveMovement : IMovementBehavior
{
    private float speed;
    private float frequency;
    private float amplitude;
    private Vector3 startPos;
    private float timeElapsed = 0f; // متغير لتتبع الوقت المنقضي

    public SinWaveMovement(float speed, float freq, float amp)
    {
        this.speed = speed;
        frequency = freq;
        amplitude = amp;
    }
    
    public void Move(Transform transform)
    {
        // احفظ نقطة البداية مرة واحدة فقط
        if (startPos == Vector3.zero) 
        {
            startPos = transform.position;
        }
        
        // قم بزيادة الوقت المنقضي بناءً على deltaTime
        timeElapsed += Time.deltaTime;

        // الحركة الأفقية (الموجة الجيبية)
        float x = startPos.x + (Mathf.Sin(timeElapsed * frequency) * amplitude);

        // الحركة الرأسية (مستمرة للأسفل)
        float y = transform.position.y - (speed * Time.deltaTime);

        // طبق الموضع الجديد
        transform.position = new Vector3(x, y, transform.position.z);
    }
}