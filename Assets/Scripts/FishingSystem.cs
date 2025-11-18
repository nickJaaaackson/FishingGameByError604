using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishingSystem : MonoBehaviour
{
    #region Inspector Variables
    [Header("References")]
    [SerializeField] private FishMiniGame fishMiniGamePrefab;
    [SerializeField] private Transform miniGameParent;

    [Header("UI Prefabs")]
    [SerializeField] private GameObject fishIconTemplate;
    [SerializeField] private float fishIconScale = 0.2f;
    [SerializeField] private float iconFloatHeight = 3f;
    [SerializeField] private float iconFloatDuration = 1.5f;
    #endregion

    #region Private Variables
    private FishMiniGame activeMiniGame;
    private BaitData usingBait;          
    private FishData cachedFish;         
    private float cachedWeight;          
    #endregion


    #region MiniGame Starter
    public void BeginFishingMiniGame(Player player, BaitData bait)
    {
        FishingAreaData area = GameManager.Instance.currentArea;

        if (activeMiniGame != null)
        {
            Debug.LogWarning(" Fishing MiniGame already running!");
            return;
        }

        if (area == null)
        {
            Debug.LogWarning(" No fishing area assigned!");
            player.UnfreezeAfterFishing();
            return;
        }

        if (bait == null)
        {
            Debug.LogWarning(" No bait selected!");
            player.UnfreezeAfterFishing();
            return;
        }

        // ---------------------------
        //  เลือกปลาก่อน
        // ---------------------------
        FishData fishToCatch = TryCatchFish(area, bait);
        if (fishToCatch == null)
        {
            Debug.Log("🐟 No fish took the bait...");
            player.UnfreezeAfterFishing();
            return;
        }

        // ---------------------------
        // เก็บข้อมูลเอาไว้ก่อน (อย่าลดเหยื่อ ณ จุดนี้)
        // ---------------------------
        usingBait = bait;
        cachedFish = fishToCatch;
        cachedWeight = Random.Range(fishToCatch.minWeight, fishToCatch.maxWeight);

        Camera overlayCam = GameObject.Find("MiniGameCamera").GetComponent<Camera>();

        activeMiniGame = Instantiate(fishMiniGamePrefab);
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 10);
        activeMiniGame.transform.position = overlayCam.ScreenToWorldPoint(screenCenter);

        // ส่งปลาพร้อมน้ำหนัก
        activeMiniGame.StartMiniGame(cachedFish, player, this, cachedWeight);

        Debug.Log($"🎣 Start MiniGame: {cachedFish.fishName} | Weight {cachedWeight:F2} Kg");
    }
    #endregion


    #region MiniGame End (Called by MiniGame)
    public void OnMiniGameResult(bool success)
    {
        // ---------------------------
        //  ลดเหยื่อ ตอนมินิเกมจบเท่านั้น!
        // ---------------------------
        ConsumeBait(usingBait);

        // ---------------------------
        //  ถ้าชนะ → เก็บปลาเข้ากระเป๋า
        // ---------------------------
        if (success)
        {
            AddFishToInventory(cachedFish, cachedWeight);
        }

        // ---------------------------
        //  เช็คว่าเหยื่อชนิดนี้ยังเหลือไหม
        // ---------------------------
        bool stillHave = Inventory.Instance.items.Exists(i =>
            i is BaitItem b && b.data == usingBait
        );

        if (!stillHave)
        {
            Debug.Log("⚠ เหยื่อหมด → currentBait = null");
            Player.Instance.SetCurrentBait(null);
        }

        // ---------------------------
        //  ล้างรอบนี้
        // ---------------------------
        usingBait = null;
        cachedFish = null;
        cachedWeight = 0;

        // ---------------------------
        //  ลบมินิเกม
        // ---------------------------
        OnMiniGameFinished();
    }


    public void OnMiniGameFinished()
    {
        if (activeMiniGame != null)
        {
            Destroy(activeMiniGame.gameObject);
            activeMiniGame = null;
        }
    }
    #endregion


    #region Catch System (Weighted Random)
    public FishData TryCatchFish(FishingAreaData area, BaitData currentBait)
    {
        List<FishData> fishList = area.availableFish;
        if (fishList == null || fishList.Count == 0) return null;

        float totalChance = 0f;
        Dictionary<FishData, float> weighted = new Dictionary<FishData, float>();

        foreach (FishData fish in fishList)
        {
            bool canEat = fish.eatType switch
            {
                FishEatType.Omnivore => true,
                FishEatType.Carnivore => currentBait.baitType == BaitType.Carnivore || currentBait.baitType == BaitType.Neutral,
                FishEatType.Herbivore => currentBait.baitType == BaitType.Herbivore || currentBait.baitType == BaitType.Neutral,
                _ => false
            };

            if (!canEat) continue;

            float chance = fish.biteChance;
            if (fish.preferredBaits.Contains(currentBait))
                chance *= currentBait.bonus;

            weighted.Add(fish, chance);
            totalChance += chance;
        }

        if (weighted.Count == 0) return null;

        float rand = Random.Range(0, totalChance);
        float cumulative = 0f;

        foreach (var entry in weighted)
        {
            cumulative += entry.Value;
            if (rand <= cumulative)
                return entry.Key;
        }

        return null;
    }
    #endregion


    #region Add Fish to Inventory + Consume Bait
    public void ConsumeBait(BaitData bait)
    {
        if (bait == null) return;
        Inventory.Instance.ConsumeBait(bait);
    }

    public void AddFishToInventory(FishData fishData, float weight)
    {
        if (fishData == null) return;

        FishItem newFish = new FishItem(fishData, weight);
        Inventory.Instance.AddItem(newFish);

        Debug.Log($" Added fish: {fishData.fishName} ({weight:F1}kg)");
    }
    #endregion


    #region Spawn Floating Fish Icon
    public void SpawnFishIcon(Vector3 startPos, Sprite fishSprite)
    {
        if (fishIconTemplate == null || fishSprite == null)
        {
            Debug.LogWarning("⚠ Missing icon prefab or sprite!");
            return;
        }

        GameObject icon = Instantiate(fishIconTemplate, startPos, Quaternion.identity);

        icon.transform.localScale = Vector3.one * fishIconScale;

        SpriteRenderer sr = icon.GetComponent<SpriteRenderer>();
        sr.sprite = fishSprite;
        sr.sortingOrder = 25;

        StartCoroutine(FloatAndFade(icon));
    }

    private IEnumerator FloatAndFade(GameObject icon)
    {
        float elapsed = 0f;
        SpriteRenderer sr = icon.GetComponent<SpriteRenderer>();
        Color color = sr.color;

        Vector3 startPos = icon.transform.position;
        Vector3 endPos = startPos + Vector3.up * iconFloatHeight;

        while (elapsed < iconFloatDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / iconFloatDuration);

            icon.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
            color.a = Mathf.Lerp(1f, 0f, t);
            sr.color = color;

            yield return null;
        }

        Destroy(icon);
    }
    #endregion
}
