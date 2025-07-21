using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private LevelData_SO[] allLevels; // اسحب كل ملفات بيانات المراحل هنا
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform buttonsParent; // الكائن الذي سيحتوي على أزرار المراحل

    void Start()
    {
        // احصل على أعلى مستوى وصل إليه اللاعب (سيتم حفظه لاحقًا)
        int highestLevelUnlocked = PlayerPrefs.GetInt("HighestLevelUnlocked", 1);

        foreach (var levelData in allLevels)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, buttonsParent);
            Button levelButton = buttonObj.GetComponent<Button>();

            // تخصيص الزر (اسم، أيقونة، ...)
            buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = levelData.levelName;

            bool isUnlocked = levelData.levelIndex <= highestLevelUnlocked;
            levelButton.interactable = isUnlocked; // تفعيل الزر إذا كانت المرحلة مفتوحة

            if (isUnlocked)
            {
                // أضف حدث ضغط الزر لتحميل المرحلة
               // levelButton.onClick.AddListener(() => LoadLevel(levelData.sceneToLoad));
                levelButton.onClick.AddListener(() => LoadLevel(levelData.sceneToLoad));

            }
        }
    }

    private void LoadLevel(string sceneName)
    {
        // قبل تحميل المشهد، يمكننا حفظ بيانات المرحلة التي تم اختيارها
        // لتستخدمها اللعبة في المشهد التالي. سنستخدم Singleton أو PlayerPrefs.
        // GameSessionManager.Instance.SetCurrentLevel(levelData); // الطريقة الأفضل
        SceneManager.LoadScene(sceneName);
    }
    
    // في دالة LoadLevel داخل MainMenuManager.cs
private void LoadLevel(LevelData_SO levelData)
{
    // 1. مرر بيانات المرحلة المختارة إلى حامل البيانات
    GameDataHolder.Instance.SetSelectedLevel(levelData);

    // 2. قم بتحميل مشهد التحميل الوسيط
    SceneManager.LoadScene("Loading"); // اسم مشهد التحميل
}
}