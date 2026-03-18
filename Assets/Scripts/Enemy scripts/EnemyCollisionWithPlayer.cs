using UnityEngine;

public class EnemyCollisionWithPlayer : MonoBehaviour
{
    public float knockbackForce = 8f;
    public int enemyDamageAmount = 1;

    // --- CHANGED: Now uses OnCollisionEnter2D for physical touches! ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("The collision detected an object named: " + collision.gameObject.name);

        // We use collision.gameObject instead of collider.gameObject
        if (collision.gameObject.CompareTag("Player"))
        {
            // 1. Grab ALL the necessary components from the Player
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            PlayerData playerData = collision.gameObject.GetComponent<PlayerData>();
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

            // 2. Calculate direction
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

            // 3. Apply the physical force
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            // 4. Stun the player (pause their movement script)
            if (playerMovement != null)
            {
                playerMovement.knockbackTimer = 0.2f;
            }

            // 5. Deal Damage
            if (playerData != null)
            {
                Debug.Log("Mole: I found the PlayerData! Dealing " + enemyDamageAmount + " damage!");
                playerData.TakeDamage(enemyDamageAmount);
            }
            else
            {
                Debug.LogWarning("Mole: I hit the player, but I can't find their PlayerData script!");
            }
        }
    }
}