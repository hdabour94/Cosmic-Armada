using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipSelectionUIManager : MonoBehaviour
{
    [SerializeField] private GameObject shipButtonPrefab;
    [SerializeField] private Transform shipsParent;
    [SerializeField] private TextMeshProUGUI shipInfoText;
    [SerializeField] private Image shipIconPreview;
    [SerializeField] private Button nextButton;

        [SerializeField] private HubPlayerManager hubPlayerManager;

    private ShipData_SO[] allShips;


    private void OnEnable()
    {
        nextButton.interactable = false;
        shipInfoText.text = "Select Ship";
        shipIconPreview.sprite = null;

        foreach (Transform child in shipsParent)
            Destroy(child.gameObject);

        /*    allShips = Resources.LoadAll<ShipData_SO>("GameData/Ships");
            foreach (var ship in allShips)
            {
                var btn = Instantiate(shipButtonPrefab, shipsParent);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = ship.shipName;
                btn.GetComponentInChildren<Image>().sprite = ship.icon;
                btn.GetComponent<Button>().onClick.AddListener(() => OnShipSelected(ship));
            }*/
        var unlockedShips = GameSessionManager.Instance.UnlockedShipIDs;

        foreach (var shipID in unlockedShips)
        {
            ShipData_SO shipData = GameSessionManager.Instance.GetShipDataByID(shipID);
            if (shipData != null)
            {
                var btn = Instantiate(shipButtonPrefab, shipsParent);
                // ... (نفس الكود لتهيئة الزر) ...
                btn.GetComponent<Button>().onClick.AddListener(() => OnShipSelected(shipData));
            }
        }

    }


    private void OnShipSelected(ShipData_SO ship)
    {
        // تحقق أولاً من أن حقل "stats" ليس فارغًا لتجنب الأخطاء
        /* if (ship.stats == null)
         {
             Debug.LogError($"The ShipData_SO '{ship.shipName}' is missing a reference to its CharacterStats_SO!", ship);
             shipInfoText.text = $"Name: {ship.shipName}\nError: Stats data missing!";
             nextButton.interactable = false; // لا تسمح بالمتابعة إذا كانت البيانات ناقصة
             return;
         }

         GameDataHolder.Instance.SetSelectedShip(ship);

         // الآن، اقرأ البيانات مباشرة من الكائن "stats" المرتبط
         shipInfoText.text = $"Name: {ship.shipName}\n" +
                             $"HP: {ship.stats.maxHP}\n" +
                             $"Power: {ship.stats.strength}\n" + // "Bower" was a typo
                             $"Speed: {ship.stats.speed}";

         shipIconPreview.sprite = ship.icon;
         nextButton.interactable = true;*/
     // 1. حدّث المركبة الحالية في جلسة اللعبة
        GameSessionManager.Instance.SetCurrentShip(ship.name);

        // 2. اطلب من مدير الـ Hub تبديل المركبة في المشهد
        if (hubPlayerManager != null)
        {
            hubPlayerManager.SwapPlayerShip(ship);
        }
        
        // 3. أغلق الواجهة
        this.gameObject.SetActive(false);

}

    public void OnNextButtonClicked()
    {
        this.gameObject.SetActive(false);
        FindFirstObjectByType<MainMenuManager>().ShowLevelSelection(); // ينقلك إلى مرحلة اختيار المرحلة
    }

    public void OnBackButtonClicked()
    {
        this.gameObject.SetActive(false);
        FindFirstObjectByType<MainMenuManager>().ShowMainMenu(); // يعيدك إلى القائمة الرئيسية
    }
}
