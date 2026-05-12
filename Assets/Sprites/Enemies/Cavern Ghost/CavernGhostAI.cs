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
    public float dashSeekRange = 5f;
    public Color dashTelegraphColor = Color.white;

    [Header("Ghost Trail")]
    public Color ghostColor = new Color(0.5f, 0.5f, 1f, 0.5f); // Bluish transparent
    public float trailSpawnInterval = 0.05f;
    public float trailFadeSpeed = 3f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private EnemyKnockback knockbackComponent; // NEW

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        knockbackComponent = GetComponent<EnemyKnockback>(); // NEW

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        // NEW: If knockback is occurring, immediately cancel updating AI movement
        if (knockbackComponent != null && knockbackComponent.IsBeingKnockedBack) return;

        if (player == null || isBusy) return;

        float dist = Vector2.Distance(transform.position, player.position);

        HandleFacing();

        if (attackCount >= 2 && dist <= dashSeekRange && Time.time >= nextAttackTime)
        {
            attackCount = 0;
            StartCoroutine(DashAttackSequence());
        }
        else if (dist <= attackRange && Time.time >= nextAttackTime)
        {
            PerformNormalAttack();
        }
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
        if (isBusy) return;

        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void PerformNormalAttack()
    {
        attackCount++;
        nextAttackTime = Time.time + attackCooldown;
        anim.SetTrigger("Ghost_Attack");
        DealDamage();
    }

    IEnumerator DashAttackSequence()
    {
        isBusy = true;
        rb.linearVelocity = Vector2.zero;

        // Telegraph
        spriteRenderer.color = dashTelegraphColor;
        Vector2 dashDir = (player.position - transform.position).normalized;

        if (dashDir.x < 0) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);

        yield return new WaitForSeconds(windUpTime);

        // Dash Start
        spriteRenderer.color = originalColor;
        anim.SetTrigger("Ghost_Dash");

        float startTime = Time.time;
        float trailTimer = 0f;

        while (Time.time < startTime + dashDuration)
        {
            // NEW: Pause dash velocity if hit mid-air
            if (knockbackComponent != null && knockbackComponent.IsBeingKnockedBack)
            {
                yield return null;
                continue;
            }

            rb.linearVelocity = dashDir * dashSpeed;

            // SPAWN TRAIL
            trailTimer -= Time.deltaTime;
            if (trailTimer <= 0)
            {
                CreateAfterimage();
                trailTimer = trailSpawnInterval;
            }
            yield return null;
        }

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
        GameObject trailObj = new GameObject("GhostTrail_Instance");
        trailObj.transform.position = transform.position;
        trailObj.transform.localScale = transform.localScale;
        trailObj.transform.rotation = transform.rotation;

        SpriteRenderer tr = trailObj.AddComponent<SpriteRenderer>();
        tr.sprite = spriteRenderer.sprite;
        tr.color = ghostColor;
        tr.sortingLayerID = spriteRenderer.sortingLayerID;
        tr.sortingOrder = spriteRenderer.sortingOrder - 1;

        StartCoroutine(FadeTrail(tr));
        Destroy(trailObj, 1f);
    }

    IEnumerator FadeTrail(SpriteRenderer tr)
    {
        while (tr != null)
        {
            Color c = tr.color;
            c.a -= trailFadeSpeed * Time.deltaTime;
            tr.color = c;

            if (c.a <= 0)
            {
                Destroy(tr.gameObject);
                yield break;
            }
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