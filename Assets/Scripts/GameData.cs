//using System.Collections.Generic;
//using UnityEngine;

//public static class GameData
//{
//    public static List<Fish> allFish;
//    public static List<Bait> allBait;
//    public static List<FishingArea> allAreas;

//    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//    public static void LoadAllData()
//    {
//        allFish = new List<Fish>(Resources.LoadAll<Fish>("Data/Fish"));
//        allBait = new List<Bait>(Resources.LoadAll<Bait>("Data/Bait"));
//        allAreas = new List<FishingArea>(Resources.LoadAll<FishingArea>("Data/Area"));

//        Debug.Log($"✅ Loaded {allFish.Count} fish, {allBait.Count} bait, {allAreas.Count} areas from Resources!");
//    }
//}