using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;       // Invisible point near the player (where the sword hits)
    public Transform crosshair;         // The visual crosshair object

    public float attackDistance = 1.2f;
    public float attackRange = 0.6f;
    public int attackDamage = 1;
    public float attackRate = 2f;

    private float nextAttackTime = 0f;

    [Header("Enemy Targeting")]
    public LayerMask enemyLayers;

    void Start()
    {
        // Hide the default Windows mouse cursor
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
        if (attackPoint == null || Mouse.current == null) return;

        // Get mouse position
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mousePosition.z = 0f;

        // --- REMOVED CROSSHAIR.POSITION TELEPORTATION HERE ---
        // We let CrosshairScript handle the visual position to prevent jittering.

        // Calculate direction for the actual (invisible) hit point
        Vector3 direction = (mousePosition - transform.position).normalized;
        attackPoint.position = transform.position + direction * attackDistance;

        // Flip the player based on mouse side
        if (mousePosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (mousePosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void Attack()
    {
        // Logic for hitting enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
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