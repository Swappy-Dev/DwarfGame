using UnityEngine;
public class GoldPickup : MonoBehaviour
{
    public int goldAmount = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("GoldPickup touched by: " + collision.gameObject.name);
        if (collision.CompareTag("Player"))
        {
            SoundManager.Instance.Play("pickupCoin");
            PlayerGold playerGold = collision.GetComponent<PlayerGold>();
            if (playerGold != null)
            {
                playerGold.AddGold(goldAmount);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("PlayerGold script nerastas ant Player!");
            }
        }
    }
}