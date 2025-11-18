using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject playerPrefab;
    [SerializeField] Transform StartPos;
    
    public FishingAreaData[] allAreas;

    private bool hasGivenStarterItem =false;
   
    [Header("Current Game State")]
    public FishingAreaData currentArea;
   

    private void Awake()
    {
        SpawnPlayerIfNeeded();
        LoadBaitDatabase();
        LoadAreas();
        
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnsceneLoaded;
    }


    private void SpawnPlayerIfNeeded()
    {
        
        Player existingPlayer = FindAnyObjectByType<Player>();

        if (existingPlayer == null)
        {
            Debug.Log("➡️ ไม่มี Player ในซีนนี้ กำลังสร้างใหม่");
            GameObject p = Instantiate(playerPrefab);

            DontDestroyOnLoad(p);
        }
        else
        {
            Debug.Log("✔ พบ Player แล้ว ไม่ต้องสร้างใหม่");
        }
    }
    //private void GiveStarterItems()
    //{
    //    if(hasGivenStarterItem) return;
    //    hasGivenStarterItem = true;

    //    if (baitDatabase == null || baitDatabase.Count == 0)
    //    {
    //        Debug.LogWarning("⚠ No bait found in Resources/BaitData!");
    //        return;
    //    }


    //    Inventory.Instance.AddItem(new BaitItem(baitDatabase[0], 5));

    //    if (baitDatabase.Count > 1)
    //       Inventory.Instance.AddItem(new BaitItem(baitDatabase[1], 3));

    //}
    private List<BaitData> baitDatabase;

    void LoadBaitDatabase()
    {
        baitDatabase = new List<BaitData>();

        
        BaitData[] loaded = Resources.LoadAll<BaitData>("Bait");

        foreach (var bait in loaded)
            baitDatabase.Add(bait);

        Debug.Log($"🎣 Loaded {baitDatabase.Count} baits from Resources.");
    }
    private void LoadAreas()
    {
        allAreas = Resources.LoadAll<FishingAreaData>("Area");

    }

    void OnsceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawn = GameObject.Find("PlayerSpawnPoint");
        if (spawn != null && Player.Instance != null)
        {
            Player.Instance.transform.position = spawn.transform.position;
        }
        else
        {
            Debug.LogWarning("No PlayerSpawnPoint found in scene" + scene.name);
        }
    }
    public void SetCurrentArea(FishingAreaData area)
    {
        currentArea = area;
        Debug.Log($"🌊 Current Area: {area.areaName}");
    }
    public void LockPlayerControll()
    {
        Player.Instance.SetMovementEnable( false );
        Debug.Log("It work");
    }
    public void UnlockPlayerControll()
    {
        Player.Instance.SetMovementEnable(true);
    }
}
