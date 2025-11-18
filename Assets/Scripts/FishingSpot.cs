using UnityEngine;

public class FishingSpot : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player.Instance.SetCanFish(true);
        }
    }
}
