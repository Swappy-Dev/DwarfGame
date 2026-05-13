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
    public float knockbackStrength = 7f;
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
    }
    void Attack()
    {
        SoundManager.Instance.Play("Swing");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D collider in hitColliders)
        {
            if (!collider.isTrigger) continue;
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
            EnemyKnockback knockback = collider.GetComponent<EnemyKnockback>();
            if (knockback != null)
            {
                knockback.ApplyKnockback(transform.position, knockbackStrength);
            }
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