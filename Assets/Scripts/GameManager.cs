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

    //private bool hasGivenStarterItem =false;

    public enum FishingEvent
    {
        None,
        Storm
    }
   
    [Header("Current Game State")]
    public FishingAreaData currentArea;

    [Header("Fishing Event")]
    public FishingEvent currentEvent = FishingEvent.None;
    public bool isStorm => currentEvent == FishingEvent.Storm;
   
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

    private void Update()
    {
        HUDManager.Instance.RefreshQuests();
    }
    private void SpawnPlayerIfNeeded()
    {
        
        Player existingPlayer = FindAnyObjectByType<Player>();

        if (existingPlayer == null)
        {
           
            GameObject p = Instantiate(playerPrefab);

            DontDestroyOnLoad(p);
        }
        
    }
    
    private List<BaitData> baitDatabase;

    void LoadBaitDatabase()
    {
        baitDatabase = new List<BaitData>();

        
        BaitData[] loaded = Resources.LoadAll<BaitData>("Bait");

        foreach (var bait in loaded)
            baitDatabase.Add(bait);

       
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
        Debug.Log($" Current Area: {area.areaName}");
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
    public void RollFishingEvent()
    {
        int roll = UnityEngine.Random.Range(0, 100);
        if ( roll < 25 )
        {
            currentEvent =FishingEvent.Storm;

        }
        else
        {
            currentEvent = FishingEvent.None;
        }
    }
    public void MoreMoney()
    {
        Player.Instance.AddMoney(1000);
        AudioManager.Instance.PlaySFX("Buy_Sell");
    }

}
