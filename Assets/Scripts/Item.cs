using UnityEngine;

[System.Serializable]   
public class Item 
{
    public string itemName;
    public int amount;
    public Sprite icon;
    public float price;
    public Item(string name,Sprite icon, int amount = 1, float price = 0)
    {
        this.itemName = name;
        this.icon = icon;
        this.amount = amount;
        this.price = price;
    }
    public virtual Sprite GetIcon()
    {
        return icon;
    }
}
