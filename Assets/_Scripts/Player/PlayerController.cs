using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ControlType controlType = ControlType.Keyboard;
    public enum ControlType { Keyboard, Mouse }

    [SerializeField] private float moveSpeed = 8f;
    
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 keyboardInput;
    private Vector3 targetPosition;
    private bool isPointerInputActive;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        targetPosition = transform.position;
    }

    void Update()
    {
        // مدخلات الكيبورد
        keyboardInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
        
        // مدخلات الماوس/اللمس
        isPointerInputActive = false;
        
        #if UNITY_STANDALONE || UNITY_EDITOR
        if (controlType == ControlType.Mouse && Input.GetMouseButton(0))
        {
            targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            isPointerInputActive = true;
        }
        #endif

        #if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            targetPosition = mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            targetPosition.z = 0;
            isPointerInputActive = true;
        }
        #endif
    }

    private void FixedUpdate()
    {
        if (isPointerInputActive)
        {
            Vector2 newPos = Vector2.MoveTowards(
                rb.position, 
                targetPosition, 
                moveSpeed * Time.fixedDeltaTime
            );
            rb.MovePosition(newPos);
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = keyboardInput * moveSpeed;
        }
    }
}