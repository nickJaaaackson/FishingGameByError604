using UnityEngine;

public class FishItem :Item
{
   public FishData fishData;
    public float weight;
    public FishItem(FishData data, float  weight)
    : base(data.fishName, data.icon, 1,data.pricePerKg)
    {
        this.fishData = data;
        this.weight = weight;
    }
    public override Sprite GetIcon()
    {
        return fishData.icon; 
    }
    public float GetSellPrice()
    {
        return fishData.pricePerKg * weight;    
    }
}
