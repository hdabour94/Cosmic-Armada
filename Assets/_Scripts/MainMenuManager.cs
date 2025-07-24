using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("المشاهد Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject shipSelectionPanel;
    [SerializeField] private GameObject levelSelectionPanel;

    [Header("اختيار المرحلة")]
    [SerializeField] private LevelData_SO[] allLevels;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform buttonsParent;

    private void Start()
    {
        ShowMainMenu(); // تأكد أن الواجهة تبدأ من القائمة الرئيسية
    }

    // ========== واجهة القائمة الرئيسية ==========
    public void OnPlayButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        shipSelectionPanel.SetActive(true);
    }

    public void OnStoreButtonClicked()
    {
        // أضف فتح واجهة المتجر هنا عند الحاجة
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    // ========== واجهة اختيار المركبة ==========
    public void ShowLevelSelection()
    {
        shipSelectionPanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
        LoadAvailableLevels();
    }

    // ========== واجهة اختيار المرحلة ==========
    private void LoadAvailableLevels()
    {
        foreach (Transform child in buttonsParent)
            Destroy(child.gameObject);

        int highestLevelUnlocked = PlayerPrefs.GetInt("HighestLevelUnlocked", 1);

        foreach (var levelData in allLevels)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, buttonsParent);
            Button levelButton = buttonObj.GetComponent<Button>();

            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = levelData.levelName;

            bool isUnlocked = levelData.levelIndex <= highestLevelUnlocked;
            levelButton.interactable = isUnlocked;

            if (isUnlocked)
            {
                //levelButton.onClick.AddListener(() => LoadLevel(levelData));
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

    private void LoadLevel(LevelData_SO levelData)
    {
        GameDataHolder.Instance.SetSelectedLevel(levelData);
        SceneManager.LoadScene("Loading"); // اسم مشهد التحميل
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        shipSelectionPanel.SetActive(false);
        levelSelectionPanel.SetActive(false);
    }
}


/*using UnityEngine;
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
    GameDataHolder.Instance.SetSelectedLevel(levelData);  // ✅ تم إصلاحها
    SceneManager.LoadScene("Loading"); // يمكنك تغيير اسم المشهد حسب الموجود فعليًا
}
}*/