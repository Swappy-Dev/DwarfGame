using UnityEngine;
using System.Collections;

public class BOSS_AI : MonoBehaviour
{
    [Header("Boso nustatymai")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float phaseTwoThreshold = 0.5f;

    [Header("Judėjimas")]
    public float moveSpeed = 2.5f;
    public float dashSpeed = 15f;
    public float chaseRange = 12f;
    public float attackRange = 2.5f;

    [Header("Atakos")]
    public float attackCooldown = 2f;
    public int projectilesInPhaseTwo = 5;
    public GameObject projectilePrefab;

    private enum BossState { Idle, Chasing, Attacking, Dashing, Stunned, Dead }
    private BossState currentState = BossState.Idle;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyKnockback knockbackComponent;

    private bool isDead = false;
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
        if (player == null || isDead) return;

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
                    if (Random.value > 0.7f) StartCoroutine(DashAttack());
                    else StartCoroutine(MeleeAttack());
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (player == null || isDead || currentState != BossState.Chasing)
        {
            if (currentState != BossState.Dashing) rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * (isPhaseTwo ? moveSpeed * 1.5f : moveSpeed);

        FlipSprite(dir.x);
    }

    IEnumerator MeleeAttack()
    {
        currentState = BossState.Attacking;
        rb.linearVelocity = Vector2.zero;

        Debug.Log("Bossas puola artimoje kovoje!");
        yield return new WaitForSeconds(0.5f);

        if (isPhaseTwo) ShootProjectiles();

        cooldownTimer = attackCooldown;
        if (!isDead) currentState = BossState.Chasing;
    }

    IEnumerator DashAttack()
    {
        currentState = BossState.Dashing;
        Vector2 dashDir = ((Vector2)player.position - (Vector2)transform.position).normalized;

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.4f);

        rb.linearVelocity = dashDir * dashSpeed;
        yield return new WaitForSeconds(0.3f);

        rb.linearVelocity = Vector2.zero;
        currentState = BossState.Stunned;
        yield return new WaitForSeconds(1f);

        cooldownTimer = attackCooldown;
        if (!isDead) currentState = BossState.Chasing;
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

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (!isPhaseTwo && currentHealth <= maxHealth * phaseTwoThreshold)
            StartPhaseTwo();

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        currentState = BossState.Dead;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        StartCoroutine(ShowVictoryAfterDelay(1.2f));
    }

    IEnumerator ShowVictoryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Ieško ir išjungtų objektų per Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        Transform panel = canvas.transform.Find("VictoryPanel");

        if (panel != null)
        {
            panel.gameObject.SetActive(true);
            Debug.Log("VictoryPanel parodytas!");
        }
        else
        {
            Debug.LogError("VictoryPanel nerastas Canvas viduje!");
        }

        Destroy(gameObject, 0.5f);
    }

    void StartPhaseTwo()
    {
        isPhaseTwo = true;
        moveSpeed *= 1.2f;
        attackCooldown *= 0.8f;
        GetComponent<SpriteRenderer>().color = Color.red;
        Debug.Log("BOSS ENRAGED!");
    }

    private void FlipSprite(float x)
    {
        if (Mathf.Abs(x) > 0.1f)
        {
            float currentScale = 3.5f;
            transform.localScale = new Vector3(x < 0 ? currentScale : -currentScale, currentScale, currentScale);
        }
    }
}