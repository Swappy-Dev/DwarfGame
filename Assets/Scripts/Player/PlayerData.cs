using UnityEngine;
using System;

public class PlayerData : MonoBehaviour
{
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerDeath;

    public int maxHealth = 10;
    public int currentHealth;

    public float invincibilityTime = 0.8f;
    private float invincibilityTimer = 0f;

    [SerializeField] private Animator animator;
    private const string flashRedAnim = "FlashRed";

    void Start()
    {
        currentHealth = maxHealth;
    }

    // --- NEW: This allows the Dash script to trigger I-Frames ---
    public void SetInvincibility(float duration)
    {
        invincibilityTimer = duration;
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
        // If the dash (or a recent hit) set the timer, we take 0 damage
        if (invincibilityTimer > 0)
        {
            return;
        }

        invincibilityTimer = invincibilityTime;
        currentHealth -= damage;

        Debug.Log("Player hit for " + damage + " damage. Remaining: " + currentHealth);

        OnPlayerDamaged?.Invoke();
        animator.SetTrigger(flashRedAnim);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("You're dead");
            OnPlayerDeath?.Invoke();
        }
    }
}