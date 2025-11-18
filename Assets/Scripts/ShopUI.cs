using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject shopWindow;
    public TextMeshProUGUI moneyText;

    [Header("Tabs")]
    public Button btnShop;
    public GameObject tabShop;
    public Button tabUpgrade;
    public Button tabQuest;
    public Button tabSell;

    [Header("Content")]
    public Transform shopListContainer;

    [Header("Prefab")]
    public GameObject itemPrefab;   // ShopItemPrefab

    Shop shop;  // reference จาก Shop.cs

    public void Open()
    {
        shopWindow.SetActive(true);
    }
    public void Close()
    {
        shopWindow.SetActive(false);
        
    }
    //-----------------------------------
    public void InitTabs(Shop shopRef)
    {
        shop = shopRef;

        // ผูกปุ่มแท็บ
        btnShop.onClick.AddListener(() => shop.ShowShopTab());
        tabUpgrade.onClick.AddListener(() => Debug.Log("Upgrade Tab ยังไม่ทำ"));
        tabQuest.onClick.AddListener(() => Debug.Log("Quest Tab ยังไม่ทำ"));
        tabSell.onClick.AddListener(() => Debug.Log("Sell Tab ยังไม่ทำ"));

        RefreshMoney();
    }

    //-----------------------------------
    // อัปเดตเงินใน UI
    //-----------------------------------
    public void RefreshMoney()
    {
        moneyText.text = "Money: " + Player.Instance.money;
    }

    //-----------------------------------
    // แสดงรายการเหยื่อทั้งหมด
    //-----------------------------------
    public void ShowBaitList(List<BaitData> baits)
    {
        ClearList();

        foreach (var bait in baits)
        {
            GameObject item = Instantiate(itemPrefab, shopListContainer);

            // หา UI ลูก
            var img = item.transform.Find("Image").GetComponent<Image>();
            var txtName = item.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var txtPrice = item.transform.Find("Price").GetComponent<TextMeshProUGUI>();
            var btnAction = item.transform.Find("Btn_Action").GetComponent<Button>();
            var btnText = btnAction.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

            // ตั้งค่า
            img.sprite = bait.icon;
            txtName.text = bait.baitName;
            txtPrice.text = bait.price.ToString();
            btnText.text = "Buy";

            // ซื้อ
            btnAction.onClick.AddListener(() =>
            {
                shop.BuyBait(bait);
            });
        }
    }

    //-----------------------------------
    void ClearList()
    {
        foreach (Transform child in shopListContainer)
            GameObject.Destroy(child.gameObject);
    }
}