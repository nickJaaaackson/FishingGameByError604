using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
public class Shop : MonoBehaviour, IInteractable
{
    public static Shop Instance;

    [Header("UI Reference")]
    public ShopUI shopUI;

    [Header("Loaded Data")]
    public List<BaitData> baitList = new List<BaitData>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadAllItems();
        shopUI.InitTabs(this);
    }

    public void Interact(Player player)
    {
        shopUI.Open();
    }
    //-----------------------------------
    // โหลดรายการไอเท็มจาก Resources
    //-----------------------------------
    void LoadAllItems()
    {
        baitList = new List<BaitData>(Resources.LoadAll<BaitData>("Bait"));
        Debug.Log("Loaded Baits : " + baitList.Count);
    }

    //-----------------------------------
    // เรียกตอนกดแท็บ Shop
    //-----------------------------------
    public void ShowShopTab()
    {
        shopUI.tabShop.SetActive(true);
        shopUI.ShowBaitList(baitList);
    }

    //-----------------------------------
    // ซื้อไอเท็ม
    //-----------------------------------
    public void BuyBait(BaitData bait)
    {
        int price = bait.price;

        if (Player.Instance.money < price)
        {
            Debug.Log(" เงินไม่พอ!");
            return;
        }

        Player.Instance.money -= price;

        // เพิ่มของเข้ากระเป๋า
        Inventory.Instance.AddItem(new BaitItem(bait, 1));

        Debug.Log($"ซื้อเหยื่อ {bait.baitName} ราคา {price} บาท");

        // อัปเดต UI
        shopUI.RefreshMoney();
    }
}