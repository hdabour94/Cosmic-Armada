// DungeonGate.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonGate : MonoBehaviour, IInteractable
{
    [SerializeField] private LevelData_SO levelToLoad; // <<<--- اسحب بيانات المرحلة هنا

    // تطبيق الواجهة
    public string InteractionPrompt => $"Enter {levelToLoad.levelName}";

    public void Interact()
    {
        Debug.Log($"Entering dungeon: {levelToLoad.levelName}");

        // استخدم النظام الذي أنشأته بالفعل!
        GameDataHolder.Instance.SetSelectedLevel(levelToLoad);
        
        // لاحقًا: احفظ موقع اللاعب الحالي في العالم المفتوح للعودة إليه
        // GameDataHolder.Instance.SetLastHubPosition(transform.position);

        SceneManager.LoadScene("Game Scene"); // أو اسم مشهد التحميل الخاص بك
    }
}