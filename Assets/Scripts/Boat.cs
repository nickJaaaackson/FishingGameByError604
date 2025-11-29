using UnityEngine;

public class Boat : MonoBehaviour, IInteractable
{

    [Header("Boat Settings")]

    public int maxLevel = 3;
    public int currentLevel = 1;  
    public float interactDistance = 2f;       

    [Header("References")]
    public Player player;                    
    public FishingAreaUI fishingAreaUI;       

   public void Interact(Player player)
    {
        fishingAreaUI.OpenUI(this);
    }


    void Update()
    {
        if (fishingAreaUI == null)
        {
            fishingAreaUI = FindAnyObjectByType<FishingAreaUI>();
        }
    }
    public int  GetUpgradePrice()
    {
        return 1000 * currentLevel;
    }

    public bool UpgradeBoat()
    {
        if (currentLevel >= maxLevel) return false;
        currentLevel++;
        return true;
    }




}
