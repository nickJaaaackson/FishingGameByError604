using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtTarget;
    public TextMeshProUGUI txtAmount;
    public TextMeshProUGUI txtReward;

    public Button btnAction;       // ปุ่มเดียว ใช้ทั้ง Accept / Submit
    public TextMeshProUGUI btnActionText;

    public Button btnMore;
    public GameObject detailPanel;
    public TextMeshProUGUI txtDetail;

    private QuestData quest;

    private LayoutElement layoutElement;
    private float baseHeight = 250f;
    private float detailHeight = 300f;


    //===============================

    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
    }
    public void Setup(QuestData q)
    {
        quest = q;

        txtName.text = q.questName;
        txtTarget.text = q.type.ToString();
        txtReward.text = $"{q.rewardMoney} $";

        // เลือกแสดงจำนวนตามประเภทเควสต์
        switch (q.type)
        {
            case QuestType.CatchSpecificFish:
                string fishName = q.targetFish != null ? q.targetFish.fishName : "fish";
                txtTarget.text = $"Catch {fishName}";
                txtAmount.text = q.targetAmount.ToString();
                break;

            case QuestType.CatchTotalWeight:
                txtAmount.text ="CatchFish Total" + q.targetWeight + " kg";
                break;

            case QuestType.CatchByRarity:
                txtTarget.text = $"Catch {q.targetRarity} fish";
                txtAmount.text = q.targetRarityAmount.ToString();
                break;
        }

        txtDetail.text = q.description;

        // เช็คว่า user รับเควสต์นี้อยู่ไหม
        if (QuestSystem.Instance.activeQuests.Contains(q))
        {
            btnActionText.text = "Submit";
        }
        else
        {
            btnActionText.text = "Accept";
        }

        btnAction.onClick.RemoveAllListeners();
        btnAction.onClick.AddListener(OnActionClicked);

        btnMore.onClick.RemoveAllListeners();
        btnMore.onClick.AddListener(ToggleDetail);
    }

    //===============================
    private void OnActionClicked()
    {
        // ยังไม่ได้รับเควสต์ → รับเควสต์
        if (!QuestSystem.Instance.activeQuests.Contains(quest))
        {
            QuestSystem.Instance.AcceptCurrentQuest(quest.type);
            btnActionText.text = "Submit";
        }
        else
        {
            // รับแล้ว → ส่งเควสต์
            QuestSystem.Instance.SubmitQuest(quest);
            btnActionText.text = "Accept";   // เควสต์ใหม่จะมาแทนหลังรีเฟรช UI
        }

        // ให้ ShopUI เรียกรีเฟรชหน้าต่างเควสต์ใหม่
        FindAnyObjectByType<Shop>().ShowQuestTab();
    }

    //===============================
    private void ToggleDetail()
    {
        bool show = !detailPanel.activeSelf;
        detailPanel.SetActive(show);
        btnMore.GetComponentInChildren<TextMeshProUGUI>().text = show ? "Hide" : "More";

        layoutElement.preferredHeight = show ? (baseHeight + detailHeight) : baseHeight;

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);

    }
}
