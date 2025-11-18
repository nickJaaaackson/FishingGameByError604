using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AreaCardUI : MonoBehaviour
{
    [SerializeField] private Image areaImage;
    [SerializeField] private TextMeshProUGUI areaName;
    [SerializeField] private TextMeshProUGUI areaLore;

    private FishingAreaData data;
    private FishingAreaUI ui;

    public void Setup(FishingAreaData newData, FishingAreaUI newUI)
    {
        data = newData;
        ui = newUI;

        areaImage.sprite = data.previewImage;
        areaName.text = data.areaName;
        areaLore.text = data.loreText;

        GetComponent<Button>().onClick.AddListener(() => ui.TrySelectArea(data));
    }
}
