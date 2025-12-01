using UnityEngine;
using UnityEngine.SceneManagement;

public class FishingArea : MonoBehaviour
{
    public FishingAreaData selectedArea;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SelectArea(FishingAreaData area)
    {
        selectedArea = area;
        Debug.Log("▶ Load area : " + area.areaName);

        SceneManager.LoadScene(area.sceneName);
        AudioManager.Instance.PlaySFX("travel",0.4f);
       
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.Instance.SetCurrentArea(selectedArea);
        if(scene.name != "Dock")
        {
            GameManager.Instance.RollFishingEvent();
            if(GameManager.Instance.currentEvent==GameManager.FishingEvent.Storm)
            {
                HUDManager.Instance.ShowEvent("To day is Raining!!!",Color.red);
            }
        }
        else
        {
            GameManager.Instance.currentEvent = GameManager.FishingEvent.None;
        }
    }
}
