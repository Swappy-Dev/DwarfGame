using UnityEngine;
using System.Collections;

public class OutlawMoleAI : MonoBehaviour
{
    [Header("Movement & Range")]
    public float chaseRange = 8f;
    public float attackRange = 4f;
    public float moveSpeed = 2f;

    [Header("Timings (Match your Animation Clip lengths!)")]
    public float attackCooldown = 3f;
    public float digDownDuration = 0.8f;
    public float undergroundTime = 1.5f;
    public float digUpDuration = 1.0f;

    [Header("Combat")]
    public GameObject dynamitePrefab;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private EnemyKnockback knockbackComponent; // NEW

    private float nextAttackTime;
    private bool isDigging = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        knockbackComponent = GetComponent<EnemyKnockback>(); // NEW

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        // NEW: If being knocked back, do not execute normal state logic
        if (knockbackComponent != null && knockbackComponent.IsBeingKnockedBack) return;

        if (player == null || isDigging) return;

        float dist = Vector2.Distance(transform.position, player.position);

        FlipSprite();

        if (dist <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(PerformDigAttack());
            }
        }
        else if (dist < chaseRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            PlayAnimationIfNew("Outlaw_Mole_Idle");
        }
    }

    void MoveTowardPlayer()
    {
        PlayAnimationIfNew("Outlaw_Mole_Idle");
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    private IEnumerator PerformDigAttack()
    {
        isDigging = true;
        nextAttackTime = Time.time + attackCooldown;
        rb.linearVelocity = Vector2.zero;

        // 1. DIG DOWN
        animator.Play("Outlaw_Mole_Dig_Down");
        yield return new WaitForSeconds(digDownDuration);

        col.enabled = false;
        spriteRenderer.enabled = false;

        // 2. UNDERGROUND
        yield return new WaitForSeconds(undergroundTime);

        if (player != null)
        {
            // Reappear slightly offset from player
            Vector2 randomOffset = Random.insideUnitCircle.normalized * 2.5f;
            transform.position = (Vector2)player.position + randomOffset;
        }

        // 3. DIG UP
        spriteRenderer.enabled = true;
        animator.Play("Outlaw_Mole_Dig_Up");
        yield return new WaitForSeconds(digUpDuration);

        // 4. THROW DYNAMITE 
        if (dynamitePrefab != null && player != null)
        {
            GameObject dyn = Instantiate(dynamitePrefab, transform.position, Quaternion.identity);
            dyn.GetComponent<Dynamite>()?.Throw(player.position);
        }

        col.enabled = true;
        isDigging = false;
    }

    void FlipSprite()
    {
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void PlayAnimationIfNew(string animName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            animator.Play(animName);
        }
    }
}