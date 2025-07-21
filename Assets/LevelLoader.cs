using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    void Start()
    {
        // ابدأ بتحميل مشهد اللعبة في الخلفية
        StartCoroutine(LoadGameSceneAsync());
    }

    private System.Collections.IEnumerator LoadGameSceneAsync()
    {
        // احصل على بيانات المرحلة من حامل البيانات
    
            LevelData_SO levelToLoad = GameDataHolder.Instance.SelectedLevel;
    
        if (levelToLoad == null)
        {
            Debug.LogError("No level data selected! Returning to main menu.");
            SceneManager.LoadScene("MainMenu Scene");
            yield break;
        }

        // قم بتعيين بيانات المرحلة في GameManager
        // بما أن GameManager هو DontDestroyOnLoad، يمكننا الوصول إليه
        LevelManager.Instance.SetCurrentLevel(levelToLoad);

        // अब مشهد اللعبة الفعلي
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelToLoad.sceneToLoad);

        // انتظر حتى يكتمل التحميل
        while (!asyncLoad.isDone)
        {
            // هنا يمكنك تحديث شريط التحميل (e.g., slider.value = asyncLoad.progress)
            yield return null;
        }
    }
}