using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingAreaUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform contentParent;   
    [SerializeField] private GameObject areaCardPrefab;
    

    [Header("Popup Confirm")]
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject warnningPanel;
    [SerializeField] private TextMeshProUGUI warnningText;
    [SerializeField] private TextMeshProUGUI confirmText;

    private FishingAreaData selectedArea;
    private Boat currentBoat;
    private FishingArea fishingAreaSystem;


    private void Awake()
    {
        DontDestroyOnLoad(this);    
    }
    private void Start()
    {
        fishingAreaSystem = FindAnyObjectByType<FishingArea>();

        panel.SetActive(false);
        confirmPanel.SetActive(false);

        GenerateAreaCards();
    }

    void GenerateAreaCards()
    {
        var areas = GameManager.Instance.allAreas;

        foreach (var area in areas)
        {
            var card = Instantiate(areaCardPrefab, contentParent);
            SetupCard(card, area);
        }
    }

    void SetupCard(GameObject card, FishingAreaData area)
    {
        card.transform.Find("Image").GetComponent<Image>().sprite = area.previewImage;
        card.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = area.areaName;
        card.transform.Find("Lore").GetComponent<TextMeshProUGUI>().text = area.loreText;

        Button btn = card.GetComponent<Button>();
        btn.onClick.AddListener(() => TrySelectArea(area));
    }

    public void TrySelectArea(FishingAreaData area)
    {
        if (currentBoat == null)
        {
            Debug.LogWarning("⚠️ currentBoat is null. ต้องเรียก OpenUI(boat) ก่อน");
            return;
        }

        if (GameManager.Instance.currentArea == area)
        {
            warnningText.text = ($"You are already in {area.name}.");
            warnningPanel.SetActive(true);
            return;
        }

        selectedArea = area;

        if (currentBoat.boatLevel < area.requiredBoatLevel)
        {
            warnningText.text = $"This Level {area.requiredBoatLevel} required";
            warnningPanel.SetActive(true);
        }
        else
        {
            confirmText.text = $"Travel to {area.areaName}?";
            confirmPanel.SetActive(true);
        }
    }

    public void OnConfirmYes()
    {
        if (selectedArea == null) return;

        fishingAreaSystem.SelectArea(selectedArea);
        confirmPanel.SetActive(false);
        panel.SetActive(false);
        GameManager.Instance.UnlockPlayerControll();
    }

    public void OnConfirmNo()
    {
        confirmPanel.SetActive(false);
    }
    
    public void OnConfirmOk()
    {
        warnningPanel.SetActive(false );
    }

    public void OpenUI(Boat boat)
    {
        currentBoat = boat;
        panel.SetActive(true);
        GameManager.Instance.LockPlayerControll();
    }

    public void CloseUI()
    {
        panel.SetActive(false);
        GameManager.Instance.UnlockPlayerControll();
    }
}
