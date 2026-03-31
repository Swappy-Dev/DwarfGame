using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public Transform crosshair;

    public float attackDistance = 1.2f;
    public float attackRange = 0.6f;
    public int attackDamage = 1;
    public float attackRate = 2f;
    public float knockbackStrength = 7f; // <--- NEW: Controls how far enemies fly

    private float nextAttackTime = 0f;

    [Header("Enemy Targeting")]
    public LayerMask enemyLayers;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        AimTowardsMouse();

        if (Time.time >= nextAttackTime)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void AimTowardsMouse()
    {
        if (attackPoint == null || Mouse.current == null || Camera.main == null) return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mousePosition.z = 0f;

        Vector3 offsetFromPlayer = mousePosition - transform.position;
        Vector3 clampedOffset = Vector3.ClampMagnitude(offsetFromPlayer, attackDistance);

        attackPoint.position = transform.position + clampedOffset;

        if (crosshair != null)
        {
            crosshair.position = new Vector3(attackPoint.position.x, attackPoint.position.y, -1f);
        }

        if (clampedOffset.x < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (clampedOffset.x > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void Attack()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D collider in hitColliders)
        {
            if (!collider.isTrigger) continue;

            // 1. Handle Damage
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }

            // 2. Handle Knockback (NEW PART)
            // We only look for the knockback script on things hit in the enemyLayers
            EnemyKnockback knockback = collider.GetComponent<EnemyKnockback>();
            if (knockback != null)
            {
                // We pass our position so the enemy flies AWAY from us
                knockback.ApplyKnockback(transform.position, knockbackStrength);
            }

            // 3. Handle Breakables
            BreakObject breakable = collider.GetComponent<BreakObject>();
            if (breakable != null)
            {
                breakable.Break();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}