using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("المشاهد Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject shipSelectionPanel;
    [SerializeField] private GameObject levelSelectionPanel;

    [Header("اختيار المرحلة")]
    [SerializeField] private LevelData_SO[] allLevels;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform buttonsParent;

    void Start()
    {
        ShowMainMenu(); // تأكد أن الواجهة تبدأ من القائمة الرئيسية
        SetupResolutionOptions();
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartNewGame()
    {
        shipSelectionPanel.SetActive(true);
    }

    public void SelectShip(ShipData_SO shipData)
    {
        GameDataHolder.Instance.SetSelectedShip(shipData);
        SceneManager.LoadScene("loadin");
    }

    private void SetupResolutionOptions()
    {
        resolutionDropdown.ClearOptions();
        var resolutions = Screen.resolutions;
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            var res = resolutions[i];
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{res.width}x{res.height}"));

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
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
    public void OpenWorld()
    {
        // قبل تحميل المشهد، يمكننا حفظ بيانات المرحلة التي تم اختيارها
        // لتستخدمها اللعبة في المشهد التالي. سنستخدم Singleton أو PlayerPrefs.
        // GameSessionManager.Instance.SetCurrentLevel(levelData); // الطريقة الأفضل
        SceneManager.LoadScene("GalaxyHub_Scene");
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