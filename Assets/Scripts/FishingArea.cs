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
        // ส่งข้อมูลไป GameManager หลังโหลดเสร็จ
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.Instance.SetCurrentArea(selectedArea);
    }
}
