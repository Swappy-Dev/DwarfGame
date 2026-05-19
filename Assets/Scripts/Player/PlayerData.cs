using UnityEngine;
using System;
public class PlayerData : MonoBehaviour
{
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerDeath;
    public static event Action OnPlayerHealed;
    public int maxHealth = 10;
    public int currentHealth;
    public float invincibilityTime = 0.8f;
    private float invincibilityTimer = 0f;
    [SerializeField] private Animator animator;
    private const string flashRedAnim = "FlashRed";
    void Start()
    {
        currentHealth = maxHealth;
        invincibilityTimer = 0f;
    }
    private void OnDestroy()
    {
        OnPlayerDamaged = null;
        OnPlayerDeath = null;
        OnPlayerHealed = null;
    }
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
    public static void ClearEvents()
    {
        OnPlayerDamaged = null;
        OnPlayerDeath = null;
        OnPlayerHealed = null;
    }
    public void TakeDamage(int damage)
    {
        if (invincibilityTimer > 0)
        {
            return;
        }
        SoundManager.Instance.Play("Hurt");
        invincibilityTimer = invincibilityTime;
        currentHealth -= damage;
        Debug.Log("Player hit for " + damage + " damage. Remaining: " + currentHealth);
        OnPlayerDamaged?.Invoke();
        animator.SetTrigger(flashRedAnim);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("You're dead");
            SoundManager.Instance.Play("Death");
            OnPlayerDeath?.Invoke();
            GetComponent<PlayerDeathHandler>()?.HandleDeath();
        }
    }
    public void Heal(int amount)
    {
        if (currentHealth >= maxHealth) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("Player healed for " + amount + ". Current health: " + currentHealth);

        // Pranešame širdelėms, kad laikas persipiešti!
        OnPlayerHealed?.Invoke();
    }
}