using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;


    [Header("Effects")]

    public GameObject deathEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
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

        // Sunaikiname kirtiklį, jei toks yra
        MoleAI moleAI = GetComponent<MoleAI>();
        if (moleAI != null && moleAI.activePickaxe != null)
        {
            Destroy(moleAI.activePickaxe);
        }

        // TEISINGAS PAKEITIMAS:
        // Naikiname tik šį konkretų priešą, o ne jo tėvą (generatorių)
        Destroy(gameObject);
    }

}
