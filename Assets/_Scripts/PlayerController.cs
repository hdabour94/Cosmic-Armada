using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Control Settings")]
    [Tooltip("اختر طريقة التحكم الأساسية على الحاسوب")]
    [SerializeField] private ControlType controlType = ControlType.Keyboard;
    public enum ControlType { Keyboard, Mouse }

    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed = 8f;

    private Rigidbody2D rb;
    private Camera mainCamera;

    // متغيرات للكيبورد
    private Vector2 keyboardInput;

    // متغيرات للموبايل والماوس
    private Vector3 targetPosition;
    private bool isPointerInputActive = false; // اسم أوضح من isInputActive

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        targetPosition = transform.position; // ابدأ من الموقع الحالي
    }

    void Update()
    {
        // ==========================================================
        //  القسم 1: قراءة المدخلات
        // ==========================================================
        
        // --- قراءة مدخلات الكيبورد (دائمًا) ---
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        keyboardInput = new Vector2(moveX, moveY).normalized;
        
        // --- قراءة مدخلات الماوس أو اللمس (حسب المنصة) ---
        isPointerInputActive = false; // إعادة تعيين الحالة في كل إطار

#if UNITY_STANDALONE || UNITY_EDITOR
        // في المحرر أو على الحاسوب، تحقق من اختيار التحكم بالماوس ومن الضغط عليه
        if (controlType == ControlType.Mouse && Input.GetMouseButton(0))
        {
            targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0; // مهم جدًا في 2D
            isPointerInputActive = true;
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        // على الموبايل، تحقق من وجود لمسة
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
        // ==========================================================
        //  القسم 2: تطبيق الحركة
        // ==========================================================

        // إذا كان هناك إدخال بالماوس/اللمس، فهو له الأولوية
        if (isPointerInputActive)
        {
            // استخدم MovePosition للحركة الموجهة بالفيزياء نحو الهدف
            // هذا أفضل من Lerp في FixedUpdate لتجنب التقطيع
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            
            // أوقف أي حركة متبقية من الكيبورد
            rb.linearVelocity = Vector2.zero;
        }
        else // إذا لم يكن هناك إدخال بالماوس/اللمس، استخدم الكيبورد
        {
            rb.linearVelocity = keyboardInput * moveSpeed;
        }
    }
}