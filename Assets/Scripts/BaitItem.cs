using UnityEngine;

public class BaitItem : Item
{
    public BaitData data;

    public BaitItem(BaitData data, int amount = 1)
    : base(data.baitName, data.icon, amount,data.price)
    {
        this.data = data;
    }
    public override Sprite GetIcon()
    {
       return data.icon;
    }
  
}
