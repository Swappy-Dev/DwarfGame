using UnityEngine;
using System;

public class PlayerData : MonoBehaviour
{
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerDeath;

    public int maxHealth = 10;
    public int currentHealth;

    public float invincibilityTime = 0.8f; // Increased slightly for safety
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
        // 1. STOPS INSTANT DEATH: If we just got hit, ignore all other incoming damage
        if (invincibilityTimer > 0)
        {
            return;
        }

        // 2. Set the timer IMMEDIATELY before doing anything else
        invincibilityTimer = invincibilityTime;

        currentHealth -= damage;
        Debug.Log("Player hit for " + damage + " damage. Remaining: " + currentHealth);

        OnPlayerDamaged?.Invoke();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("You're dead");
            OnPlayerDeath?.Invoke();
        }
    }
}