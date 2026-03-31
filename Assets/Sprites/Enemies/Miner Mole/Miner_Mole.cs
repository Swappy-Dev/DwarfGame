using UnityEngine;

public class MinerMole : MonoBehaviour
{
    [Header("Movement & Range")]
    public float chaseRange = 8f;
    public float attackRange = 5f;
    public float moveSpeed = 3f;
    private float rangeBuffer = 0.75f;

    [Header("Combat")]
    public GameObject pickaxePrefab;
    public float throwDistance = 6f;
    public float repositionTime = 1.5f;
    public LayerMask obstacleLayer;
    public float wallAvoidanceDistance = 1.2f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyKnockback knockbackComponent; // Reference to our knockback script

    private enum State { Idle, Moving, Throwing, WaitingForPickaxe, Repositioning }
    private State currentState = State.Idle;

    private float repositionTimer;
    private Vector2 randomRepositionSpot;
    [HideInInspector] public GameObject activePickaxe;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        knockbackComponent = GetComponent<EnemyKnockback>(); // Link the scripts

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (animator != null) animator.Play("Miner_Mole_Idle");
    }

    void Update()
    {
        if (player == null) return;

        // --- KNOCKBACK CHECK ---
        // If we are being shoved, stop all AI logic immediately
        if (knockbackComponent != null && knockbackComponent.IsBeingKnockedBack)
            return;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                if (dist <= chaseRange - rangeBuffer) currentState = State.Moving;
                break;
            case State.Moving:
                if (dist > chaseRange + rangeBuffer) currentState = State.Idle;
                else if (dist <= attackRange && HasLineOfSight()) currentState = State.Throwing;
                break;
            case State.Throwing:
                PerformThrow();
                break;
            case State.Repositioning:
                repositionTimer -= Time.deltaTime;
                if (repositionTimer <= 0) currentState = State.Moving;
                break;
        }
    }

    void FixedUpdate()
    {
        // --- KNOCKBACK CHECK ---
        // If being knocked back, do NOT let the AI touch the Rigidbody velocity
        if (knockbackComponent != null && knockbackComponent.IsBeingKnockedBack)
            return;

        if (player == null || currentState == State.Throwing || currentState == State.WaitingForPickaxe)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 target = (currentState == State.Moving) ? (Vector2)player.position : randomRepositionSpot;
        Vector2 dir = (target - (Vector2)transform.position).normalized;

        if (Vector2.Distance(transform.position, target) > 0.3f)
        {
            FlipSprite(dir.x);
            ApplyMovementWithAvoidance(dir);
        }
        else if (currentState == State.Repositioning)
        {
            currentState = State.Moving;
        }
    }

    private void PerformThrow()
    {
        if (pickaxePrefab != null)
        {
            GameObject thrown = Instantiate(pickaxePrefab, transform.position, Quaternion.identity);
            activePickaxe = thrown;
            if (thrown.TryGetComponent(out BoomerangPickaxe script))
            {
                Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
                script.Throw(transform, (Vector2)transform.position + (dir * throwDistance));
            }
        }
        currentState = State.WaitingForPickaxe;
    }

    public void CatchPickaxe()
    {
        currentState = State.Repositioning;
        repositionTimer = repositionTime;
        randomRepositionSpot = (Vector2)transform.position + (Random.insideUnitCircle.normalized * 3f);
    }

    private void ApplyMovementWithAvoidance(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.4f, dir, wallAvoidanceDistance, obstacleLayer);
        if (hit.collider != null)
        {
            Vector2 slide = new Vector2(-hit.normal.y, hit.normal.x);
            if (Vector2.Dot(slide, dir) < 0) slide = -slide;
            dir = Vector2.Lerp(dir, slide, 0.5f).normalized;
        }
        rb.linearVelocity = dir * moveSpeed;
    }

    private bool HasLineOfSight()
    {
        if (player == null) return false;
        float dist = Vector2.Distance(transform.position, player.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (player.position - transform.position).normalized, dist, obstacleLayer);
        return hit.collider == null;
    }

    private void FlipSprite(float x)
    {
        if (Mathf.Abs(x) > 0.2f)
        {
            transform.localScale = new Vector3(x < 0 ? 1 : -1, 1, 1);
        }
    }
}