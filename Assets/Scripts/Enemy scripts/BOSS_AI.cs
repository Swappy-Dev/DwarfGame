using UnityEngine;
using System.Collections;

public class BOSS_AI : MonoBehaviour
{
    [Header("Boso nustatymai")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float phaseTwoThreshold = 0.5f; // Pereina į 2 fazę ties 50% HP

    [Header("Judėjimas")]
    public float moveSpeed = 2.5f;
    public float dashSpeed = 15f;
    public float chaseRange = 12f;
    public float attackRange = 2.5f;

    [Header("Atakos")]
    public float attackCooldown = 2f;
    public int projectilesInPhaseTwo = 5;
    public GameObject projectilePrefab; // Galima naudoti pickaxe arba specialų boso sviedinį

    private enum BossState { Idle, Chasing, Attacking, Dashing, Stunned }
    private BossState currentState = BossState.Idle;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyKnockback knockbackComponent;

    private float cooldownTimer;
    private bool isPhaseTwo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        knockbackComponent = GetComponent<EnemyKnockback>();
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Knockback patikra (perimta iš tavo pavyzdžio)
        if (knockbackComponent != null && knockbackComponent.IsBeingKnockedBack)
            return;

        float dist = Vector2.Distance(transform.position, player.position);
        cooldownTimer -= Time.deltaTime;

        LogicUpdate(dist);
    }

    private void LogicUpdate(float dist)
    {
        switch (currentState)
        {
            case BossState.Idle:
                if (dist < chaseRange) currentState = BossState.Chasing;
                break;

            case BossState.Chasing:
                if (dist <= attackRange && cooldownTimer <= 0)
                {
                    // Atsitiktinai renkasi tarp paprastos atakos ir Dash
                    if (Random.value > 0.7f) StartCoroutine(DashAttack());
                    else StartCoroutine(MeleeAttack());
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (player == null || currentState != BossState.Chasing)
        {
            if (currentState != BossState.Dashing) rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * (isPhaseTwo ? moveSpeed * 1.5f : moveSpeed);

        FlipSprite(dir.x);
    }

    // --- ATAKŲ LOGIKA ---

    IEnumerator MeleeAttack()
    {
        currentState = BossState.Attacking;
        rb.linearVelocity = Vector2.zero;

        // Čia grotum animaciją: animator.SetTrigger("Attack");
        Debug.Log("Bossas puola artimoje kovoje!");

        yield return new WaitForSeconds(0.5f); // Atakos animacijos laikas

        if (isPhaseTwo) ShootProjectiles(); // Antroje fazėje po atakos dar iššauna

        cooldownTimer = attackCooldown;
        currentState = BossState.Chasing;
    }

    IEnumerator DashAttack()
    {
        currentState = BossState.Dashing;
        Vector2 dashDir = ((Vector2)player.position - (Vector2)transform.position).normalized;

        // Trumpas pasiruošimas (Anticipation)
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.4f);

        // Pats Dashas
        rb.linearVelocity = dashDir * dashSpeed;
        yield return new WaitForSeconds(0.3f);

        rb.linearVelocity = Vector2.zero;
        currentState = BossState.Stunned; // Po dasho bossas trumpam sustingsta
        yield return new WaitForSeconds(1f);

        cooldownTimer = attackCooldown;
        currentState = BossState.Chasing;
    }

    void ShootProjectiles()
    {
        for (int i = 0; i < projectilesInPhaseTwo; i++)
        {
            float angle = i * (360f / projectilesInPhaseTwo);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Instantiate(projectilePrefab, transform.position, rotation);
        }
    }

    // --- PAGALBINĖS FUNKCIJOS ---

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (!isPhaseTwo && currentHealth <= maxHealth * phaseTwoThreshold)
        {
            StartPhaseTwo();
        }
    }

    void StartPhaseTwo()
    {
        isPhaseTwo = true;
        moveSpeed *= 1.2f;
        attackCooldown *= 0.8f;
        // Galima pakeisti spalvą ar groti pykčio animaciją
        GetComponent<SpriteRenderer>().color = Color.red;
        Debug.Log("BOSS ENRAGED!");
    }

    private void FlipSprite(float x)
    {
        if (Mathf.Abs(x) > 0.1f)
        {
            // Vietoj 1 naudojame 3.5f, kad išlaikytume boso dydį
            float currentScale = 3.5f;
            transform.localScale = new Vector3(x < 0 ? currentScale : -currentScale, currentScale, currentScale);
        }
    }
}