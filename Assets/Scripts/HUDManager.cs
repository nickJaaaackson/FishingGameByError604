using UnityEngine;
using TMPro;
using System.Collections;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("Money")]
    public TextMeshProUGUI moneyText;

    [Header("Quest Tracking")]
    public TextMeshProUGUI[] questTexts;   

    [Header("Event Popup")]
    public TextMeshProUGUI interactText;
    public TextMeshProUGUI eventText;
    public GameObject eventPanel;
    bool isShowingEvent = false;
    public float eventDuration = 2f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        eventText.gameObject.SetActive(false);
    }

    void Start()
    {
        RefreshMoney();
        RefreshQuests();
        ShowInteract(false);
    }

    // ============================================================
    // MONEY
    // ============================================================
    public void RefreshMoney()
    {
        moneyText.text = Player.Instance.money.ToString("N0") + " G";
    }

    // ============================================================
    // QUESTS
    // ============================================================
    public void RefreshQuests()
    {
        var qs = QuestSystem.Instance;

      
        for (int i = 0; i < questTexts.Length; i++)
            questTexts[i].text = "";

        
        for (int i = 0; i < qs.activeQuests.Count && i < questTexts.Length; i++)
        {
            QuestData q = qs.activeQuests[i];
            questTexts[i].text = FormatQuestText(q);
        }
    }
    private string FormatQuestText(QuestData q)
    {
        switch (q.type)
        {
            case QuestType.CatchSpecificFish:
                return $"Catch {q.targetFish.fishName} {QuestSystem.Instance.GetProgressText(q)}";

            case QuestType.CatchTotalWeight:
                return $"Catch Fish Total Weight {QuestSystem.Instance.GetProgressText(q)} KG";

            case QuestType.CatchByRarity:
                return $"Catch {q.targetRarity} Fish {QuestSystem.Instance.GetProgressText(q)}";

            default:
                return "";
        }
    }



    
    public void ShowEvent(string msg, Color color)
    {
        if (isShowingEvent) return;
        StartCoroutine(ShowEventRoutine(msg, color));
    }

    IEnumerator ShowEventRoutine(string msg, Color color)
    {
        isShowingEvent = true;

        eventText.text = msg;
        eventText.color = color;
        eventPanel.SetActive(true);
        eventText.gameObject.SetActive(true);

        yield return new WaitForSeconds(eventDuration);

        eventPanel.SetActive(false);
    }
    public void ShowInteract(bool isStay)
    {
        if(isStay)
        {
            interactText.gameObject.SetActive(true);
        }
        else
        {
            interactText.gameObject.SetActive(false);
        }


    }
}
