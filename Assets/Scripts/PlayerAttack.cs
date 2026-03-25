using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;       // Nematomas taškas prie pat žaidėjo (kur kerta kardas)
    public Transform crosshair;         // NAUJA: Vizualus taikiklis, kuris laisvai sekios pelę!

    public float attackDistance = 1.2f;
    public float attackRange = 0.6f;
    public int attackDamage = 1;
    public float attackRate = 2f;

    private float nextAttackTime = 0f;

    [Header("Enemy Targeting")]
    public LayerMask enemyLayers;

    void Start()
    {
        // Paslepiame Windows pelę
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

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mousePosition.z = 0f;

        // 1. Vizualus taikiklis laisvai nuskrenda tiksliai ten, kur yra pelė!
        if (crosshair != null)
        {
            crosshair.position = mousePosition;
        }

        // 2. Tikrasis (nematomas) smūgio taškas lieka arti žaidėjo, bet atsisuka į pelės pusę
        Vector3 direction = (mousePosition - transform.position).normalized;
        attackPoint.position = transform.position + direction * attackDistance;

        // 3. Apverčiame žaidėją
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