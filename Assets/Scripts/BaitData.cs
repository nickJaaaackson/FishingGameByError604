using System.Data;
using UnityEngine;

public enum BaitType { Herbivore, Carnivore, Neutral }

[CreateAssetMenu(fileName = "New Bait", menuName = "Fishing/Bait Data")]
public class BaitData : ScriptableObject
{
    public string baitName;
    public Sprite icon;
    public BaitType baitType;  
    public float bonus;
    public int price = 10;
    [TextArea]
    public string description;
}
