using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    #region Instance
    public static QuestSystem Instance;
    private void Awake() => Instance = this;
    #endregion

    #region Queues
    [Header("Quest Queues (Type A / B / C)")]
    public Queue<QuestData> queueA = new();
    public Queue<QuestData> queueB = new();
    public Queue<QuestData> queueC = new();

    [Header("Current Quests (UI Display)")]
    public QuestData currentA;
    public QuestData currentB;
    public QuestData currentC;

    [Header("Active Quests (Accepted by Player)")]
    public List<QuestData> activeQuests = new();
    #endregion

    private void Start()
    {
        LoadAllQuests();
        InitializeQuestQueues();
    }

    #region Load Quests
    void LoadAllQuests()
    {
        // โหลดทุก QuestData ใน Resources/Quests/*
        QuestData[] allQuests = Resources.LoadAll<QuestData>("Quests");

        foreach (var q in allQuests)
        {
            switch (q.type)
            {
                case QuestType.CatchSpecificFish:
                    queueA.Enqueue(q);
                    break;

                case QuestType.CatchTotalWeight:
                    queueB.Enqueue(q);
                    break;

                case QuestType.CatchByRarity:
                    queueC.Enqueue(q);
                    break;
            }
        }

        Debug.Log($"Loaded Quests → A:{queueA.Count}  B:{queueB.Count}  C:{queueC.Count}");
    }
    #endregion

    #region Initialize
    public void InitializeQuestQueues()
    {
        currentA = queueA.Count > 0 ? queueA.Dequeue() : null;
        currentB = queueB.Count > 0 ? queueB.Dequeue() : null;
        currentC = queueC.Count > 0 ? queueC.Dequeue() : null;
    }
    #endregion

    #region Accept Quest
    public void AcceptCurrentQuest(QuestType type)
    {
        QuestData q = type switch
        {
            QuestType.CatchSpecificFish => currentA,
            QuestType.CatchTotalWeight => currentB,
            QuestType.CatchByRarity => currentC,
            _ => null
        };

        if (q == null) return;

        if (!activeQuests.Contains(q))
        {
            activeQuests.Add(q);
            Debug.Log("Accepted quest: " + q.questName);
        }
    }
    #endregion

    #region Check & Submit
    public bool CheckQuestProgress(QuestData quest)
    {
        var inv = Inventory.Instance.items;

        switch (quest.type)
        {
            case QuestType.CatchSpecificFish:
                int count = 0;
                foreach (var item in inv)
                    if (item is FishItem f && f.fishData == quest.targetFish)
                        count++;
                return count >= quest.targetAmount;

            case QuestType.CatchTotalWeight:
                float totalW = 0f;
                foreach (var item in inv)
                    if (item is FishItem f2)
                        totalW += f2.weight;
                return totalW >= quest.targetWeight;

            case QuestType.CatchByRarity:
                int rarCount = 0;
                foreach (var item in inv)
                    if (item is FishItem f3 && f3.fishData.rarity == quest.targetRarity)
                        rarCount++;
                return rarCount >= quest.targetRarityAmount;
        }

        return false;
    }

    public void SubmitQuest(QuestData quest)
    {
        if (!CheckQuestProgress(quest))
        {
            Debug.Log("ยังไม่ครบ ส่งเควสต์ไม่ได้");
            return;
        }

        RemoveFishForQuest(quest);

        Player.Instance.money += quest.rewardMoney;
        activeQuests.Remove(quest);

        // ดึงคิวถัดไป
        switch (quest.type)
        {
            case QuestType.CatchSpecificFish:
                currentA = queueA.Count > 0 ? queueA.Dequeue() : null;
                break;

            case QuestType.CatchTotalWeight:
                currentB = queueB.Count > 0 ? queueB.Dequeue() : null;
                break;

            case QuestType.CatchByRarity:
                currentC = queueC.Count > 0 ? queueC.Dequeue() : null;
                break;
        }
    }
    #endregion

    #region Remove Fish
    void RemoveFishForQuest(QuestData quest)
    {
        var inv = Inventory.Instance.items;

        switch (quest.type)
        {
            case QuestType.CatchSpecificFish:
                int removed = 0;
                for (int i = inv.Count - 1; i >= 0 && removed < quest.targetAmount; i--)
                {
                    if (inv[i] is FishItem f && f.fishData == quest.targetFish)
                    {
                        inv.RemoveAt(i);
                        removed++;
                    }
                }
                break;

            case QuestType.CatchTotalWeight:
                float needW = quest.targetWeight;
                for (int i = inv.Count - 1; i >= 0 && needW > 0; i--)
                {
                    if (inv[i] is FishItem f)
                    {
                        needW -= f.weight;
                        inv.RemoveAt(i);
                    }
                }
                break;

            case QuestType.CatchByRarity:
                int needR = quest.targetRarityAmount;
                for (int i = inv.Count - 1; i >= 0 && needR > 0; i--)
                {
                    if (inv[i] is FishItem f && f.fishData.rarity == quest.targetRarity)
                    {
                        needR--;
                        inv.RemoveAt(i);
                    }
                }
                break;
        }
    }
    #endregion
}
