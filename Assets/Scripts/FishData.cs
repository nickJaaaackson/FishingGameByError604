using System.Collections.Generic;
using UnityEngine;

public enum FishEatType { Herbivore, Carnivore, Omnivore }
public enum Rarity {Common,Uncommon,Rare,Epic,Legendary}

[CreateAssetMenu(fileName = "New Fish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishName;
    public Sprite icon;

    [Header("Stats")]
    public float minWeight;
    public float maxWeight;
    public int pricePerKg;
    public Rarity rarity;
    public float biteChance;

    [Header("Behavior")]
    public FishEatType eatType;          
    public List<BaitData> preferredBaits; 
}
