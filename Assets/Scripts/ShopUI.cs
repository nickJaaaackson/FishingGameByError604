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
    public Transform questListContainer;
    public Transform sellListContainer;

    [Header("Prefabs")]
    public GameObject shopItemPrefab;
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

    public void ShowQuestList()
    {
        foreach(Transform c in questListContainer)
            Destroy(c.gameObject);
        var qs = QuestSystem.Instance;
        List<QuestData> list = new List<QuestData>();
        if(qs.currentA != null) list.Add(qs.currentA);
        if(qs.currentB != null) list.Add(qs.currentB);
        if(qs.currentC != null) list.Add(qs.currentC);
        foreach(var q in list)
        {
            GameObject ui =
                Instantiate(questSlotPrefab, questListContainer);
            ui.GetComponent<QuestSlotUI>().Setup(q);
        }
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