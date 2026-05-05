using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Effects")]
    public GameObject deathEffect;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        DamagePopupManager.Instance.Show(damage, transform.position + Vector3.up);

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

        Destroy(gameObject);
    }
}