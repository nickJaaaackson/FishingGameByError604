using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fishing Area", menuName = "Fishing/Fishing Area Data")]
public class FishingAreaData : ScriptableObject
{
    [Header("Basic Info")]
    public string areaName;
    public string sceneName;

    [TextArea(2, 4)]
    public string loreText;        

    public Sprite previewImage;    
    public int requiredBoatLevel = 1;  

    [Header("Available Fish")]
    public List<FishData> availableFish;
}
