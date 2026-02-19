using UnityEngine;

public class EnemyCollisionWithPlayer : MonoBehaviour
{
    public float knockbackForce = 8f;
    public int enemyDamageAmount = 10;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("The trigger detected an object named: " + collider.gameObject.name);

        if (collider.gameObject.CompareTag("Player"))
        {
            // 1. Grab ALL the necessary components from the Player
            Rigidbody2D playerRb = collider.gameObject.GetComponent<Rigidbody2D>();
            PlayerData playerData = collider.gameObject.GetComponent<PlayerData>();

            
            PlayerMovement playerMovement = collider.gameObject.GetComponent<PlayerMovement>();

            // 2. Calculate direction
            Vector2 knockbackDirection = (collider.transform.position - transform.position).normalized;

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
                playerData.TakeDamage(enemyDamageAmount);
            }
        }
    }
}