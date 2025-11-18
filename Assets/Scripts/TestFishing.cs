//using UnityEngine;

//public class TestFishing : MonoBehaviour
//{
//    public FishMiniGame miniGame; // ลากออบเจกต์ FishMiniGame จากฉากมาใส่ใน Inspector

//    void Start()
//    {
//        Debug.Log("🎣 Fishing Test Ready. Click to start fishing!");
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.E))
//        {
//            StartFishing();
//        }
//    }

//    void StartFishing()
//    {
//        // ✅ ดึงพื้นที่จาก GameData โดยชื่อ
//        FishingArea selectedArea = GameData.fishingAreas.Find(a => a.name == "Whitewave Shore");

//        if (selectedArea == null)
//        {
//            Debug.LogError("❌ ไม่พบพื้นที่ Whitewave Shore ใน GameData!");
//            return;
//        }

//        // ✅ ใช้เหยื่อกำหนดเอง เช่น Shrimp
//        Bait selectedBait = GameData.baits.Find(b => b.name == "FishMeat");

//        if (selectedBait == null)
//        {
//            Debug.LogError("❌ ไม่พบเหยื่อ Shrimp ใน GameData!");
//            return;
//        }

//        Debug.Log($"🌊 ตกปลาที่: {selectedArea.name} | ใช้เหยื่อ: {selectedBait.name}");

//        // ✅ เรียกระบบตกปลา
//        Fish hookedFish = FishingSystem.TryCatchFish(selectedArea, selectedBait);

//        if (hookedFish != null)
//        {
//            Debug.Log($"🐟 คุณเกี่ยว {hookedFish.name} ได้! เริ่มมินิเกม...");
//            miniGame.StartMiniGame(hookedFish);
//        }
//        else
//        {
//            Debug.Log("🪱 ไม่มีปลากินเหยื่อในรอบนี้...");
//        }
//    }
//}
