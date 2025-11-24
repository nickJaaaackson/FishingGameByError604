using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using System;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
public class InventoryUI : MonoBehaviour
{
   public static InventoryUI Instance;
    [SerializeField] GameObject panel;
    [SerializeField] Transform slotParent;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] GameObject closeButton;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            Toggle();
        }
    }

    private void Toggle()
    {
        if(panel.activeSelf)
        {
            panel.SetActive(false);
            GameManager.Instance.UnlockPlayerControll();
        }
        else
        {
            GameManager.Instance.LockPlayerControll();
            RefeshUI();
        }
    }

    private void RefeshUI()
    {
        panel.SetActive(true);

        foreach (Transform t in slotParent)
            Destroy(t.gameObject);

        int maxUnlockedSlots = Inventory.Instance.maxSlots; // เช่น 10 ช่องแรกใช้งานได้

        for (int i = 0; i < 30; i++)  // 30 ช่อง UI เต็ม
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);

            // หา object ย่อย
            var iconObj = slot.transform.Find("Icon");
            var nameObj = slot.transform.Find("Name");
            var amountObj = slot.transform.Find("Amount");
            var highlightObj = slot.transform.Find("Highlight");
            var lockObj = slot.transform.Find("Lock");

            var icon = iconObj.GetComponent<Image>();
            var nameText = nameObj.GetComponent<TextMeshProUGUI>();
            var amountText = amountObj.GetComponent<TextMeshProUGUI>();

            // ถ้าเป็นช่องที่ล็อค
            if (i >= maxUnlockedSlots)
            {
                icon.enabled = false;
                nameText.text = "";
                amountText.text = "";
                lockObj.gameObject.SetActive(true);
                continue;
            }

            // ช่องที่ใช้งานได้
            lockObj.gameObject.SetActive(false);

            if (i < Inventory.Instance.items.Count)
            {
                var item = Inventory.Instance.items[i];

                icon.sprite = item.GetIcon();
                nameText.text = item.itemName;
                amountText.text = "x" + item.amount;


                if (item is BaitItem bait)
                {
                    // 🎯 เปรียบเทียบแบบถูกต้อง (BaitData กับ BaitData)
                    bool isSelected = Player.Instance.currentBait == bait.data;
                    highlightObj.gameObject.SetActive(isSelected);

                    Button btn = slot.GetComponent<Button>();
                    btn.onClick.AddListener(() =>
                    {
                        Player.Instance.SetCurrentBait(bait.data);
                        Debug.Log("🎣 เลือกเหยื่อ: " + bait.data.baitName);
                        RefeshUI();   // refresh highlight ให้ช่องอื่นอัปเดตด้วย
                    });
                }
                else
                {
                    highlightObj.gameObject.SetActive(false);
                }

            }
            else
            {
                // ช่องว่าง
                icon.enabled = false;
                nameText.text = "";
                amountText.text = "";
                highlightObj.gameObject.SetActive(false);
            }
        }
    }

    public void CloseUI()
    {
        panel.SetActive(false);
        GameManager.Instance.UnlockPlayerControll();
    }
}
