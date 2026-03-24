using UnityEngine;

public class CavernGhostAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseRange = 8f;
    public float attackRange = 1.1f;
    public LayerMask obstacleLayer; // Set to 'Obstacles' in Inspector

    [Header("Combat")]
    public int damage = 1;
    public float attackCooldown = 2.0f; // Set this to 2 or 3 for a slow attack
    private float nextAttackTime;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Find the player by Tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // 1. Flip Sprite to face player
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        // 2. State Logic
        if (dist <= attackRange)
        {
            // STOP moving and ATTACK (Even if overlapping/on top)
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= nextAttackTime)
            {
                Attack();
            }
        }
        else if (dist < chaseRange)
        {
            // CHASE the player
            MoveWithWallSliding();
        }
        else
        {
            // IDLE if player is too far
            rb.linearVelocity = Vector2.zero;
        }
    }

    void MoveWithWallSliding()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        // Raycast to detect walls (matches Mole logic)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f, obstacleLayer);

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

    void Attack()
    {
        nextAttackTime = Time.time + attackCooldown;
        anim.SetTrigger("Ghost_Attack");

        // Deal damage logic
        PlayerData data = player.GetComponent<PlayerData>();
        if (data != null)
        {
            data.TakeDamage(damage);
            Debug.Log("Ghost attacked player!");
        }
    }
}