using UnityEngine;
using System.Collections;

public class BOSS_AI : MonoBehaviour
{
    [Header("Boso nustatymai")]
    public float phaseTwoThreshold = 0.5f;

    [Header("Judėjimas")]
    public float moveSpeed = 2.5f;
    public float dashSpeed = 15f;
    public float chaseRange = 12f;
    public float attackRange = 2.5f;

    [Header("Atakos")]
    public float attackCooldown = 2f;

    private enum BossState { Idle, Chasing, Attacking, Dashing, Stunned, Dead }
    private BossState currentState = BossState.Idle;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyKnockback knockbackComponent;

    private bool isDead = false;
    private float cooldownTimer;
    private bool isPhaseTwo = false;

    [HideInInspector] public GameObject victoryPanel;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        knockbackComponent = GetComponent<EnemyKnockback>();

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

    public void CheckPhaseTransition(float currentHealth, float maxHealth)
    {
        if (!isPhaseTwo && currentHealth <= maxHealth * phaseTwoThreshold)
        {
            StartPhaseTwo();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        currentState = BossState.Dead;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Paslepiam boso vizualus ir išjungiam jo susidūrimus
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Sustabdom žaidėją
        DisablePlayerControl();

        // Paleidžiame iškart pergalės langą
        StartCoroutine(ShowVictoryAfterDelay(0f));
    }

    private void DisablePlayerControl()
    {
        if (player != null)
        {
            // 1. Išjungiam pagrindinį judėjimą
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null) movement.enabled = false;

            // 2. Išjungiam Brūkšnį (Dash)
            PlayerDash dash = player.GetComponent<PlayerDash>();
            if (dash != null) dash.enabled = false;

            // 3. Išjungiam Ataką
            PlayerAttack attack = player.GetComponent<PlayerAttack>();
            if (attack != null) attack.enabled = false;

            // 4. Sustabdom žaidėjo fiziką
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.bodyType = RigidbodyType2D.Kinematic;
            }

            Cursor.visible = true; // Padaro pelę matomą

            Debug.Log("Žaidėjo kontrolė išjungta, pelės žymeklis įjungtas!");
        }
    }

    IEnumerator ShowVictoryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Debug.Log("VictoryPanel sėkmingai parodytas per ObjectGenerator!");
        }
        else
        {
            Debug.LogError("BOSS_AI negavo VictoryPanel nuorodos iš ObjectGenerator!");
        }

        Destroy(gameObject);
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