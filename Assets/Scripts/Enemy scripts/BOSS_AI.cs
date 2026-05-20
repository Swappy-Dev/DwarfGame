using UnityEngine;
using System.Collections;

public class BOSS_AI : MonoBehaviour
{
    [Header("Šviesų generavimo nustatymai")]
    public GameObject lightPrefab;
    public int lightCount = 4;
    public float spawnRadius = 3f;

    [Header("Boso nustatymai")]
    public float phaseTwoThreshold = 0.5f;

    [Header("Judėjimas")]
    public float dashSpeed = 15f;
    public float chaseRange = 12f;

    [Header("Atakos Cooldown")]
    public float attackCooldown = 2f;

    [Header("Shockwave Atakos Nustatymai")]
    public GameObject shockwavePrefab;
    public float shockwaveChargeTime = 0.6f;

    [Header("Cone Atakos Nustatymai")]
    public GameObject conePrefab;
    public float coneChargeTime = 0.5f;

    private enum BossState { Idle, Chasing, Dashing, Shockwave, ConeAttack, Stunned, Dead }
    private BossState currentState = BossState.Idle;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyKnockback knockbackComponent;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;
    private float cooldownTimer;
    private bool isPhaseTwo = false;

    [HideInInspector] public GameObject victoryPanel;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        knockbackComponent = GetComponent<EnemyKnockback>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        SpawnLightsAroundBoss();
    }

    private void SpawnLightsAroundBoss()
    {
        if (lightPrefab == null)
        {
            Debug.LogWarning("Light Prefab nepriskirtas!");
            return;
        }

        for (int i = 0; i < lightCount; i++)
        {
            float angle = i * Mathf.PI * 2f / lightCount;
            float x = Mathf.Cos(angle) * spawnRadius;
            float y = Mathf.Sin(angle) * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(x, y, 0f);
            Instantiate(lightPrefab, spawnPosition, Quaternion.identity);
        }
    }

    void Update()
    {
        if (player == null || isDead) return;

        if (knockbackComponent != null && knockbackComponent.IsBeingKnockedBack)
            return;

        float dist = Vector2.Distance(transform.position, player.position);
        cooldownTimer -= Time.deltaTime;

        if (currentState == BossState.Idle || currentState == BossState.Chasing)
        {
            Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            FlipSprite(directionToPlayer.x);
        }

        LogicUpdate(dist);
    }

    private void LogicUpdate(float dist)
    {
        if (currentState == BossState.Chasing && cooldownTimer <= 0)
        {
            int attackChoice = Random.Range(0, 3);

            if (attackChoice == 0)
                StartCoroutine(DashAttack());
            else if (attackChoice == 1)
                StartCoroutine(ShockwaveAttack());
            else
                StartCoroutine(ConeAttack());
        }

        if (currentState == BossState.Idle && dist < chaseRange)
            currentState = BossState.Chasing;
    }

    void FixedUpdate()
    {
        if (currentState != BossState.Dashing)
            rb.linearVelocity = Vector2.zero;
    }

    IEnumerator DashAttack()
    {
        currentState = BossState.Dashing;
        Vector2 dashDir = ((Vector2)player.position - (Vector2)transform.position).normalized;

        rb.linearVelocity = Vector2.zero;

        // ✅ Dash trigger
        if (animator != null) animator.SetTrigger("Dash");
        if (spriteRenderer != null) spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.4f);

        if (spriteRenderer != null) spriteRenderer.color = Color.white;

        float currentDashSpeed = isPhaseTwo ? dashSpeed * 1.5f : dashSpeed;
        rb.linearVelocity = dashDir * currentDashSpeed;
        yield return new WaitForSeconds(0.3f);

        rb.linearVelocity = Vector2.zero;
        currentState = BossState.Stunned;
        yield return new WaitForSeconds(1f);

        ResetAfterAttack();
    }

    IEnumerator ShockwaveAttack()
    {
        currentState = BossState.Shockwave;
        rb.linearVelocity = Vector2.zero;

        // ✅ Pound trigger (charge fazė)
        if (animator != null) animator.SetTrigger("Pound");
        if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 0.6f, 0f);

        yield return new WaitForSeconds(shockwaveChargeTime);

        if (spriteRenderer != null) spriteRenderer.color = Color.white;

        if (shockwavePrefab != null)
        {
            GameObject wave = Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
            if (isPhaseTwo) wave.transform.localScale *= 1.5f;
        }

        currentState = BossState.Stunned;
        yield return new WaitForSeconds(0.8f);

        ResetAfterAttack();
    }

    IEnumerator ConeAttack()
    {
        currentState = BossState.ConeAttack;
        rb.linearVelocity = Vector2.zero;

        // ✅ Slam trigger
        if (animator != null) animator.SetTrigger("Slam");
        if (spriteRenderer != null) spriteRenderer.color = new Color(0f, 0.5f, 1f);

        yield return new WaitForSeconds(coneChargeTime);

        if (spriteRenderer != null) spriteRenderer.color = Color.white;

        if (conePrefab != null)
        {
            Vector2 dirToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);
            GameObject cone = Instantiate(conePrefab, transform.position, spawnRotation);

            if (isPhaseTwo) cone.transform.localScale *= 1.3f;
        }
        else
        {
            Debug.LogWarning("Cone Prefab nepriskirtas!");
        }

        currentState = BossState.Stunned;
        yield return new WaitForSeconds(0.8f);

        ResetAfterAttack();
    }

    private void ResetAfterAttack()
    {
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        cooldownTimer = isPhaseTwo ? attackCooldown * 0.8f : attackCooldown;
        if (!isDead) currentState = BossState.Chasing;
    }

    public void CheckPhaseTransition(float currentHealth, float maxHealth)
    {
        if (!isPhaseTwo && currentHealth <= maxHealth * phaseTwoThreshold)
            StartPhaseTwo();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        currentState = BossState.Dead;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (spriteRenderer != null) spriteRenderer.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        DisablePlayerControl();
        StartCoroutine(ShowVictoryAfterDelay(0f));
    }

    private void DisablePlayerControl()
    {
        if (player != null)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null) movement.enabled = false;

            PlayerDash dash = player.GetComponent<PlayerDash>();
            if (dash != null) dash.enabled = false;

            PlayerAttack attack = player.GetComponent<PlayerAttack>();
            if (attack != null) attack.enabled = false;

            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.bodyType = RigidbodyType2D.Kinematic;
            }

            Cursor.visible = true;
        }
    }

    IEnumerator ShowVictoryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (victoryPanel != null)
            victoryPanel.SetActive(true);
        else
            Debug.LogError("VictoryPanel nepriskirtas!");

        Destroy(gameObject);
    }

    void StartPhaseTwo()
    {
        isPhaseTwo = true;
        Debug.Log("BOSS ENRAGED! Antra fazė!");
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