using UnityEngine;

public enum QuestType
{
    CatchSpecificFish,   // ประเภท A — จับปลาชนิดเดียวตามจำนวน
    CatchTotalWeight,    // ประเภท B — จับน้ำหนักรวม
    CatchByRarity        // ประเภท C — จับปลาตามความหายาก
}

[CreateAssetMenu(fileName = "Quest", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public string questName;

    public QuestType type;

    [TextArea]
    public string description;

    public int rewardMoney;

    // Type A
    public FishData targetFish;
    public int targetAmount;

    // Type B
    public float targetWeight;

    // Type C
    public Rarity targetRarity;
    public int targetRarityAmount;
}