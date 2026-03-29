using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Svarbu: pašalinau nenaudojamą naudojimą
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;       // Invisible point near the player (where the sword hits)
    public Transform crosshair;         // The visual crosshair object

    // Šis atstumas dabar yra MAKSIMALUS galimas atakos atstumas
    public float attackDistance = 1.2f;
    public float attackRange = 0.6f;    // Smūgio apskritimo spindulys
    public int attackDamage = 1;
    public float attackRate = 2f;

    private float nextAttackTime = 0f;

    [Header("Enemy Targeting")]
    public LayerMask enemyLayers;

    void Start()
    {
        // Paslepiame numatytąjį pelės žymeklį
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

        // 1. Gauname pelės poziciją pasaulyje
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mousePosition.z = 0f;

        // 2. Apskaičiuojame vektorių nuo žaidėjo iki pelės
        Vector3 offsetFromPlayer = mousePosition - transform.position;

        // --- ŠITA DALIS YRA PAKEISTA ---
        // 3. Apribojame šį vektorių, kad jis neviršytų attackDistance
        // Vector3.ClampMagnitude palieka vektorių tokį patį, jei jis trumpesnis nei attackDistance,
        // bet sutrumpina jį iki attackDistance, jei jis ilgesnis.
        Vector3 clampedOffset = Vector3.ClampMagnitude(offsetFromPlayer, attackDistance);

        // 4. Nustatome attackPoint poziciją (žaidėjas + apribotas vektorius)
        attackPoint.position = transform.position + clampedOffset;

        // --- OPTIMIZUOTAS KRYŽIUKO JUDĖJIMAS ---
        // Kadangi crosshair turi sekti attackPoint, tiesiog nustatome jo poziciją
        if (crosshair != null)
        {
            // Pridedame mažą Z poslinkį, kad crosshair visada būtų virš visko
            crosshair.position = new Vector3(attackPoint.position.x, attackPoint.position.y, -1f);
        }

        // --- APERŠKIMO (FLIP) LOGIKA ---
        // Atkreipk dėmesį: flipped žaidėjas pagal attackPoint, o ne tiesiogiai pelę,
        // tai padeda išvengti nemalonaus drebėjimo, kai pelė yra labai arti žaidėjo centro.
        if (clampedOffset.x < -0.1f) // Pridėjau mažą "deadzone", kad mažiau drebėtų
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
        // Naudojame OverlapCircleAll, nes jis gerai dera su attackPoint
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D collider in hitColliders)
        {
            // Tikriname tik Triggerius
            if (!collider.isTrigger) continue;

            // Priešas
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                continue;
            }

            // Sulaužomas objektas
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