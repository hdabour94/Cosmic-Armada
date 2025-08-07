// ShipSwapStation.cs
using UnityEngine;

public class ShipSwapStation : MonoBehaviour, IInteractable
{
    // مرجع لواجهة المستخدم التي ستظهر
    [SerializeField] private ShipSelectionUIManager shipSelectionUI;

    public string InteractionPrompt => "Change Ship";

    public void Interact()
    {
        Debug.Log("Opening Ship Selection...");
        // قم بتفعيل واجهة المستخدم الخاصة باختيار المركبة
        if (shipSelectionUI != null)
        {
            shipSelectionUI.gameObject.SetActive(true);
        }
    }
}