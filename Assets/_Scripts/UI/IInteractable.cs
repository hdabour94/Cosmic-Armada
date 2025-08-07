// IInteractable.cs
public interface IInteractable
{
    string InteractionPrompt { get; } // النص الذي سيظهر للاعب (مثل "Enter Dungeon")
    void Interact();
}