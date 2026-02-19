using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

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
