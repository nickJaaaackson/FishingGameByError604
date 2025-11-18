using UnityEngine;

public class Boat : MonoBehaviour, IInteractable
{
    
    [Header("Boat Settings")]
    public int boatLevel = 1;                
    public float interactDistance = 2f;       

    [Header("References")]
    public Player player;                    
    public FishingAreaUI fishingAreaUI;       

   public void Interact(Player player)
    {
        fishingAreaUI.OpenUI(this);
    }


    private void Start()
    {
        //if(fishingAreaUI == null)
        //{
        //    fishingAreaUI = FindAnyObjectByType<FishingAreaUI>();
        //}
    }
    void Update()
    {
        if (fishingAreaUI == null)
        {
            fishingAreaUI = FindAnyObjectByType<FishingAreaUI>();
        }
    }

   
    

    
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        playerInRange = true;
    //        player = collision.GetComponent<Player>();
    //        Debug.Log("🎣 Player entered boat area");
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        playerInRange = false;
    //        player = null;
    //        Debug.Log("👋 Player left boat area");
    //    }
    //}
    
}
