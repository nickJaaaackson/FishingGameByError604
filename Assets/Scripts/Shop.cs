using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    public static Shop Instance;

    [Header("Reference")]
    public ShopUI shopUI;

    [Header("Data List")]
    public List<BaitData> baitList;

    public void Interact(Player player)
    {
        OpenShop();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        baitList = new List<BaitData>(Resources.LoadAll<BaitData>("Bait"));
        shopUI.InitTabs(this);
    }

    //=============================================
    public void ShowShopTab()
    {
        shopUI.ShowTab(TabType.Shop);
        shopUI.ShowBaitList(baitList);
    }

    public void ShowSellTab()
    {
        shopUI.ShowTab(TabType.Sell);
        shopUI.ShowSellList();
    }

    public void ShowUpgradeTab()
    {
        shopUI.ShowTab(TabType.Upgrade);
        shopUI.ShowUpgradeList();
    }

    public void ShowQuestTab()
    {
        shopUI.ShowTab(TabType.Quest);
        shopUI.ShowQuestList();
    }

    //=============================================
    public void BuyBait(BaitData bait)
    {
        int price = bait.price;

        if (Player.Instance.money < price)
        {
            AudioManager.Instance.PlaySFX("Error");
            return;
        }

        Player.Instance.money -= price;
        Inventory.Instance.AddItem(new BaitItem(bait, 1));
        AudioManager.Instance.PlaySFX("Buy_Sell");
        shopUI.RefreshMoney();
    }

    public void SellFish(FishItem fish)
    {
        float price = fish.GetSellPrice();
        Player.Instance.money += Mathf.FloorToInt(price);
        AudioManager.Instance.PlaySFX("Buy_Sell");
        Inventory.Instance.RemoveItem(fish);

        shopUI.ShowSellList();
        shopUI.RefreshMoney();
    }

    //=============================================
    public void OpenShop()
    {
        shopUI.Open();
        ShowShopTab();
    }

    public void CloseShop()
    {
        shopUI.Close();
    }
}