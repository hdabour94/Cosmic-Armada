// PlayerInteractor.cs
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private IInteractable currentInteractable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // تحقق مما إذا كان الكائن الذي دخلنا نطاقه قابل للتفاعل
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            currentInteractable = interactable;
            // لاحقًا: أظهر واجهة مستخدم للتفاعل (e.g., UIManager.ShowInteractionPrompt(interactable.InteractionPrompt))
            Debug.Log("Can interact with: " + interactable.InteractionPrompt);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out _))
        {
            currentInteractable = null;
            // لاحقًا: أخفِ واجهة المستخدم
        }
    }

    void Update()
    {
        // تفاعل عند الضغط على زر (مثلاً 'E')
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
}