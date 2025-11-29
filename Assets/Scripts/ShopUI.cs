using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum TabType
{
    Shop,
    Upgrade,
    Quest,
    Sell
}

public class ShopUI : MonoBehaviour
{
    [Header("Shop Window")]
    public GameObject shopWindow;
    public TextMeshProUGUI moneyText;

    [Header("Tabs Setup")]
    public List<TabButton> tabs;

    [Header("Content Parent")]
    public Transform shopListContainer;
    public Transform upgradeListContainer;
    public Transform questListContainer;
    public Transform sellListContainer;

    [Header("Prefabs")]
    public GameObject shopItemPrefab;
    public GameObject upgradeSlotPrefab;
    public GameObject questSlotPrefab;
    public GameObject fishItemPrefab;

    private Shop shop;

    //===============================================================
    public void InitTabs(Shop shopRef)
    {
        shop = shopRef;

        foreach (var t in tabs)
        {
            TabType captured = t.type;
            t.button.onClick.AddListener(() => shopUI_OnTabClicked(captured));
        }

        RefreshMoney();
    }

    private void shopUI_OnTabClicked(TabType tab)
    {
        ShowTab(tab);

        switch (tab)
        {
            case TabType.Shop:
                shop.ShowShopTab();
                break;

            case TabType.Sell:
                shop.ShowSellTab();
                break;

            case TabType.Upgrade:
                shop.ShowUpgradeTab();
                break;

            case TabType.Quest:
                shop.ShowQuestTab();
                break;
        }
    }

    //===============================================================
    public void ShowTab(TabType tab)
    {
        foreach (var t in tabs)
            t.panel.SetActive(false);

        tabs.Find(x => x.type == tab).panel.SetActive(true);
    }

    //===============================================================
    public void RefreshMoney()
    {
        moneyText.text = "Money: " + Player.Instance.money;
    }

    //===============================================================
    public void ShowBaitList(List<BaitData> baitList)
    {
        foreach (Transform c in shopListContainer)
            Destroy(c.gameObject);

        foreach (var bait in baitList)
        {
            GameObject ui = Instantiate(shopItemPrefab, shopListContainer);

            var img = ui.transform.Find("Image").GetComponent<Image>();
            var txtName = ui.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var txtPrice = ui.transform.Find("Price").GetComponent<TextMeshProUGUI>();
            var btn = ui.transform.Find("Btn_Action").GetComponent<Button>();
            var btnTxt = btn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

            img.sprite = bait.icon;
            txtName.text = bait.baitName;
            txtPrice.text = bait.price.ToString();
            btnTxt.text = "Buy";

            btn.onClick.AddListener(() => shop.BuyBait(bait));
        }
    }

    public Sprite iconBoat;
    public Sprite iconHook;
    public Sprite iconLine;
    public Sprite iconBag;
    public void ShowUpgradeList()
    {
        Debug.Log(">>> OPenUbgradeUI<<<");
        // ล้างก่อน
        foreach (Transform c in upgradeListContainer)
            Destroy(c.gameObject);

        // เรียกตัวหลัก
        var p = Player.Instance;
        var boat = FindAnyObjectByType<Boat>();
        var rod = FindAnyObjectByType<FishingRod>();
        var inv = Inventory.Instance;

        // ===== สร้างช่อง Boat =====
        CreateUpgradeSlot(
            iconSprite: iconBoat,
            price: boat.GetUpgradePrice(),
            info: $"Boat Lv {boat.currentLevel}/{boat.maxLevel}",
            onClick: () =>
            {
               int price = boat.GetUpgradePrice();
                if (p.money >= boat.GetUpgradePrice() && boat.UpgradeBoat())
                {
                    p.money -= price;
                    AudioManager.Instance.PlaySFX("Buy_Sell");
                    RefreshMoney();
                    ShowUpgradeList();
                }
                else { AudioManager.Instance.PlaySFX("Error"); }
               
            }
        );

       
        CreateUpgradeSlot(
            iconSprite: iconHook,
            price: rod.GetHookUpgradePrice(),
            info: $"Hook Lv {rod.hookLevel}/{rod.hookMaxLevel}",
            onClick: () =>
            {
                if (p.money >= rod.GetHookUpgradePrice() && rod.UpgradeHook())
                {
                    p.money -= rod.GetHookUpgradePrice();
                    AudioManager.Instance.PlaySFX("Buy_Sell");
                    RefreshMoney();
                    ShowUpgradeList();
                }
                else { AudioManager.Instance.PlaySFX("Error"); }
            }
        );

        // ===== สร้างช่อง Line =====
        CreateUpgradeSlot(
            iconSprite: iconLine,
            price: rod.GetLineUpgradePrice(),
            info: $"Line Lv {rod.lineLevel}/{rod.lineMaxLevel}",
            onClick: () =>
            {
                if (p.money >= rod.GetLineUpgradePrice() && rod.UpgradeLine())
                {
                    p.money -= rod.GetLineUpgradePrice();
                    AudioManager.Instance.PlaySFX("Buy_Sell");
                    RefreshMoney();
                    ShowUpgradeList();
                }
                else { AudioManager.Instance.PlaySFX("Error"); }
            }
        );

        // ===== Bag Upgrade =====
        CreateUpgradeSlot(
            iconSprite : iconBag,
            price: 1000,
            info: "+10 Slots",
            onClick: () =>
            {
                if (p.money >= 1000 && inv.UpgradeBag())
                {
                    p.money -= 1000;
                    AudioManager.Instance.PlaySFX("Buy_Sell");
                    RefreshMoney();
                    ShowUpgradeList();
                }
                else { AudioManager.Instance.PlaySFX("Error"); }
            }
        );

    }


    public void ShowQuestList()
    {
        
        
            Debug.Log("=== ShowQuestList() START ===");

            // 1) ตรวจว่า container หายมั้ย
            if (questListContainer == null)
            {
                Debug.LogError("❌ questListContainer = NULL !!");
                return;
            }

            Debug.Log("✔ questListContainer = OK, Active = " + questListContainer.gameObject.activeInHierarchy);

            // 2) ล้างของเดิม
            foreach (Transform c in questListContainer)
            {
                Debug.Log(" - Destroy old slot: " + c.name);
                Destroy(c.gameObject);
            }

            // 3) ดึงระบบเควส
            var qs = QuestSystem.Instance;
            if (qs == null)
            {
                Debug.LogError("❌ QuestSystem.Instance = NULL !!");
                return;
            }

            Debug.Log("✔ QuestSystem = OK");

            // 4) ตรวจเควสแต่ละอัน
            Debug.Log($" currentA = {qs.currentA}");
            Debug.Log($" currentB = {qs.currentB}");
            Debug.Log($" currentC = {qs.currentC}");

            // 5) รวมเควส
            List<QuestData> list = new List<QuestData>();

            if (qs.currentA != null)
            {
                list.Add(qs.currentA);
                Debug.Log(" Add: A");
            }
            if (qs.currentB != null)
            {
                list.Add(qs.currentB);
                Debug.Log(" Add: B");
            }
            if (qs.currentC != null)
            {
                list.Add(qs.currentC);
                Debug.Log(" Add: C");
            }

            Debug.Log("Total quests to display = " + list.Count);

            // 6) ตรวจ slotPrefab
            if (questSlotPrefab == null)
            {
                Debug.LogError("❌ questSlotPrefab = NULL !!");
                return;
            }

            // 7) สร้างแต่ละสลอต + Debug
            foreach (var q in list)
            {
                GameObject ui = Instantiate(questSlotPrefab, questListContainer);
                Debug.Log(" Create Slot: " + q.questName + " → " + ui.name);

                var slotUI = ui.GetComponent<QuestSlotUI>();
                if (slotUI == null)
                {
                    Debug.LogError("❌ QuestSlotUI component not found on prefab !!");
                }
                else
                {
                    Debug.Log("✔ Setup() called");
                    slotUI.Setup(q);
                }
            }

            Debug.Log("=== ShowQuestList() END ===");
        

    }
    //===============================================================
    public void ShowSellList()
    {
        foreach (Transform c in sellListContainer)
            Destroy(c.gameObject);

        foreach (var item in Inventory.Instance.items)
        {
            if (item is FishItem fish)
            {
                GameObject ui = Instantiate(fishItemPrefab, sellListContainer);

                var img = ui.transform.Find("Image").GetComponent<Image>();
                var txtName = ui.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                var txtWeight = ui.transform.Find("Weight").GetComponent<TextMeshProUGUI>();
                var txtPrice = ui.transform.Find("Price").GetComponent<TextMeshProUGUI>();
                var btn = ui.transform.Find("Btn_Action").GetComponent<Button>();
                var btnTxt = btn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

                img.sprite = fish.fishData.icon;
                txtName.text = fish.fishData.fishName;
                txtWeight.text = fish.weight.ToString("0.0") + " kg";
                txtPrice.text = fish.GetSellPrice().ToString("0.0");
                btnTxt.text = "Sell";

                btn.onClick.AddListener(() => shop.SellFish(fish));
            }
        }
    }
    void CreateUpgradeSlot(Sprite iconSprite, int price, string info, UnityEngine.Events.UnityAction onClick)
    {
        Debug.Log(">>> CreateUbgradeSlot<<<");
        GameObject ui = Instantiate(upgradeSlotPrefab, upgradeListContainer);
        Debug.Log(">>>Slot Created<<<" + ui.name);

        var icon = ui.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();
        var txtPrice = ui.transform.Find("Price").GetComponent<TextMeshProUGUI>();
        var txtInfo = ui.transform.Find("Info").GetComponent<TextMeshProUGUI>();
        var btn = ui.transform.Find("Button").GetComponent<Button>();

        icon.sprite = iconSprite;
        txtPrice.text = price + " G";
        txtInfo.text = info;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClick);
    }



    //===============================================================
    public void Open() => shopWindow.SetActive(true);
    public void Close() => shopWindow.SetActive(false);
}

//===============================================================
[System.Serializable]
public class TabButton
{
    public TabType type;
    public Button button;
    public GameObject panel;
}