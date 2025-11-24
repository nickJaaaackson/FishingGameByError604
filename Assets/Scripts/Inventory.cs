using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public List<Item> items = new List<Item>();
    public int maxSlots = 10;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(gameObject);   
    }

    
    public void AddItem(Item item)
    {
      
        if (item is BaitItem baitItem)
        {
            foreach (var i in items)
            {
                if (i is BaitItem existingBait &&
                    existingBait.data == baitItem.data)
                {
                    existingBait.amount += baitItem.amount;
                    return;
                }
            }
        }

        if (items.Count < maxSlots)
        {
            items.Add(item);
        }
        else
        {
            Debug.LogWarning(" Inventory full! cannot add item: " + item.itemName);
        }
    }

   public void RemoveItem(Item item)
    {
        if(items.Contains(item))
        {
            items.Remove(item);
        }
    }

    public void ConsumeBait(BaitData baitData)
    {
        foreach (var i in items)
        {
            if (i is BaitItem baitItem && baitItem.data == baitData)
            {
                baitItem.amount--;

                if (baitItem.amount <= 0)
                {
                    items.Remove(baitItem);

                }

                return;
            }
        }
    }
}
