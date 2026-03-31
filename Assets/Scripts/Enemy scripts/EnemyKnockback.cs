using UnityEngine;
using System.Collections;

public class EnemyKnockback : MonoBehaviour
{
    [SerializeField] private float knockbackTime = 0.2f; // How long the push lasts
    private Rigidbody2D rb;

    // This is the "Switch". Other scripts (like AI) can check this 
    // to see if they should stop moving while the enemy is flying back.
    public bool IsBeingKnockedBack { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 hitterPosition, float strength)
    {
        if (IsBeingKnockedBack) return; // Don't stack knockbacks

        // Calculate direction: Away from the person who hit us
        Vector2 direction = (transform.position - (Vector3)hitterPosition).normalized;

        StartCoroutine(KnockbackRoutine(direction, strength));
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float strength)
    {
        IsBeingKnockedBack = true;

        // Apply the physical force
        rb.linearVelocity = direction * strength;

        yield return new WaitForSeconds(knockbackTime);

        // Stop the movement and hand control back to the AI
        rb.linearVelocity = Vector2.zero;
        IsBeingKnockedBack = false;
    }
}