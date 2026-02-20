using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public float invincibilityDuration = 1.5f; 
    private float invincibilityTimer = 0f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        if (invincibilityTimer > 0)
        {
            Debug.Log("Player is invincible and cannot take damage.");
            return;
         
        }

        currentHealth -= damage;

        invincibilityTimer = invincibilityDuration;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        Debug.Log("Ouch! Current Health: " + currentHealth);
    }

    void Die()
    {
        Debug.Log("Player has died.");
        
    }



    
}
