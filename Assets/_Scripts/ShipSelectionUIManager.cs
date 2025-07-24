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

    private ShipData_SO[] allShips;

    private void OnEnable()
    {
        nextButton.interactable = false;
        shipInfoText.text = "Select Ship";
        shipIconPreview.sprite = null;

        foreach (Transform child in shipsParent)
            Destroy(child.gameObject);

        allShips = Resources.LoadAll<ShipData_SO>("GameData/Ships");

        foreach (var ship in allShips)
        {
            var btn = Instantiate(shipButtonPrefab, shipsParent);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = ship.shipName;
            btn.GetComponentInChildren<Image>().sprite = ship.icon;
            btn.GetComponent<Button>().onClick.AddListener(() => OnShipSelected(ship));
        }
    }

    private void OnShipSelected(ShipData_SO ship)
    {
        GameDataHolder.Instance.SetSelectedShip(ship);

        shipInfoText.text = $"Name: {ship.shipName}\nHP: {ship.maxHP}\n Bower: {ship.strength}\n Speed: {ship.speed}";
        shipIconPreview.sprite = ship.icon;

        nextButton.interactable = true;
    }

    public void OnNextButtonClicked()
    {
        this.gameObject.SetActive(false);
        FindObjectOfType<MainMenuManager>().ShowLevelSelection(); // ينقلك إلى مرحلة اختيار المرحلة
    }

    public void OnBackButtonClicked()
    {
        this.gameObject.SetActive(false);
        FindObjectOfType<MainMenuManager>().ShowMainMenu(); // يعيدك إلى القائمة الرئيسية
    }
}
