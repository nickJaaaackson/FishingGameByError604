using UnityEngine;

[System.Serializable]
public class FishingRod : MonoBehaviour
{
    [Header("Hook Upgrade")]
    public int hookLevel = 1;
    public int hookMaxLevel = 5;

    public float baseHookArea = 1f;
    public float hookAreaPerLevel = 0.2f;

    [Header("Line Upgrade")]
    public int lineLevel = 1;
    public int lineMaxLevel = 5;

    public float baseTensionSpeed = 1f;
    public float tensionSpeedPerLevel = -0.1f;
    // ยิ่งอัพ → ลดช้าลง

    [Header("Price Setting")]
    public int hookBasePrice = 100;
    public int hookPriceIncrease = 100;

    public int lineBasePrice = 150;
    public int linePriceIncrease = 120;

    // ---- ค่าที่ส่งให้มินิเกมใช้ ----

    public float hookArea
    {
        get
        {
            return baseHookArea + hookAreaPerLevel * (hookLevel - 1);
        }
    }

    public float tensionSpeed
    {
        get
        {
            return baseTensionSpeed + tensionSpeedPerLevel * (lineLevel - 1);
        }
    }

    // ---- ฟังก์ชันอัพเกรด ----

    public bool UpgradeHook()
    {
        if (hookLevel >= hookMaxLevel) return false;
        hookLevel++;
        return true;
    }

    public bool UpgradeLine()
    {
        if (lineLevel >= lineMaxLevel) return false;
        lineLevel++;
        return true;
    }

    public int GetHookUpgradePrice()
    {
        return hookBasePrice + hookPriceIncrease * (hookLevel - 1);
    }

    public int GetLineUpgradePrice()
    {
        return lineBasePrice + linePriceIncrease * (lineLevel - 1);
    }
}
