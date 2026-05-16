using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Effects")]
    public GameObject deathEffect;

    private BOSS_AI bossAI;

    void Start()
    {
        currentHealth = maxHealth;
        bossAI = GetComponent<BOSS_AI>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        DamagePopupManager.Instance.Show(damage, transform.position + Vector3.up);

        
        if (bossAI != null)
        {
            bossAI.CheckPhaseTransition(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " mirė!");
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        MoleAI moleAI = GetComponent<MoleAI>();
        if (moleAI != null && moleAI.activePickaxe != null)
        {
            Destroy(moleAI.activePickaxe);
        }

        
        if (bossAI != null)
        {
            bossAI.Die();
            return; 
        }

        Destroy(gameObject);
    }
}