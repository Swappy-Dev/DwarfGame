using UnityEngine;
using System.Collections;

public class CavernGhostAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseRange = 8f;
    public float attackRange = 1.1f;
    public float stopDistance = 0.5f;
    public LayerMask obstacleLayer;

    [Header("Combat")]
    public int damage = 1;
    public float attackCooldown = 2.0f;
    private float nextAttackTime;
    private int attackCount = 0;
    private bool isBusy = false;

    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.35f;
    public float windUpTime = 0.5f;
    public float dashSeekRange = 5f; // NEW: The distance from which he will dash at you
    public Color dashTelegraphColor = Color.white;

    [Header("Ghost Trail")]
    public float trailSpawnInterval = 0.04f;
    public float trailFadeSpeed = 4f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null || isBusy) return;

        float dist = Vector2.Distance(transform.position, player.position);

        HandleFacing();

        // LOGIC BRANCH:
        // If we are ready for the 3rd attack, we don't wait to get close. 
        // We just need to be within dashSeekRange.
        if (attackCount >= 2 && dist <= dashSeekRange && Time.time >= nextAttackTime)
        {
            attackCount = 0; // Reset for next cycle
            StartCoroutine(DashAttackSequence());
        }
        // Otherwise, do normal behavior (Close-range normal attacks)
        else if (dist <= attackRange && Time.time >= nextAttackTime)
        {
            PerformNormalAttack();
        }
        // Chase behavior
        else if (dist < chaseRange && dist > stopDistance)
        {
            MoveWithWallSliding();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void HandleFacing()
    {
        // Don't flip while dashing to keep the momentum looking right
        if (isBusy) return;

        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void PerformNormalAttack()
    {
        attackCount++; // Increments after attack 1 and 2
        nextAttackTime = Time.time + attackCooldown;
        anim.SetTrigger("Ghost_Attack");
        DealDamage();
    }

    IEnumerator DashAttackSequence()
    {
        isBusy = true;
        rb.linearVelocity = Vector2.zero;

        // 1. Telegraph: Lock onto player position at THIS moment
        spriteRenderer.color = dashTelegraphColor;
        Vector2 dashDir = (player.position - transform.position).normalized;

        // Face the dash direction
        if (dashDir.x < 0) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);

        yield return new WaitForSeconds(windUpTime);

        // 2. Dash Start
        spriteRenderer.color = originalColor;
        anim.SetTrigger("Ghost_Dash");

        float startTime = Time.time;
        float trailTimer = 0f;

        while (Time.time < startTime + dashDuration)
        {
            rb.linearVelocity = dashDir * dashSpeed;

            trailTimer -= Time.deltaTime;
            if (trailTimer <= 0)
            {
                CreateAfterimage();
                trailTimer = trailSpawnInterval;
            }
            yield return null;
        }

        // 3. Recovery
        rb.linearVelocity = Vector2.zero;
        nextAttackTime = Time.time + attackCooldown;
        yield return new WaitForSeconds(0.4f);
        isBusy = false;
    }

    void MoveWithWallSliding()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.5f, obstacleLayer);

        if (hit.collider != null)
        {
            Vector2 wallNormal = hit.normal;
            Vector2 slideDir = new Vector2(-wallNormal.y, wallNormal.x);
            if (Vector2.Dot(slideDir, dir) < 0) slideDir = -slideDir;
            rb.linearVelocity = slideDir * moveSpeed;
        }
        else
        {
            rb.linearVelocity = dir * moveSpeed;
        }
    }

    void CreateAfterimage()
    {
        GameObject trailObj = new GameObject("Trail");
        trailObj.transform.position = transform.position;
        trailObj.transform.localScale = transform.localScale;

        SpriteRenderer tr = trailObj.AddComponent<SpriteRenderer>();
        tr.sprite = spriteRenderer.sprite;
        tr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.6f);
        tr.sortingOrder = spriteRenderer.sortingOrder - 1;

        Destroy(trailObj, 0.8f);
        StartCoroutine(FadeTrail(tr));
    }

    IEnumerator FadeTrail(SpriteRenderer tr)
    {
        while (tr != null)
        {
            Color c = tr.color;
            c.a -= trailFadeSpeed * Time.deltaTime;
            tr.color = c;
            if (c.a <= 0) break;
            yield return null;
        }
    }

    void DealDamage()
    {
        if (player == null) return;
        PlayerData data = player.GetComponent<PlayerData>();
        if (data != null) data.TakeDamage(damage);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isBusy && other.CompareTag("Player"))
        {
            DealDamage();
        }
    }
}