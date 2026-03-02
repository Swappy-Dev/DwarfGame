using UnityEngine;

public class MoleAI : MonoBehaviour
{
    private enum State { Moving, Throwing, WaitingForPickaxe, Repositioning }
    private State currentState;

    [Header("Movement & Vision")]
    public float moveSpeed = 3f;
    public float visionRange = 10f;
    public LayerMask obstacleLayer;
    public float wallAvoidanceDistance = 1.5f; 
    [Header("Combat")]
    public GameObject pickaxePrefab;
    public float throwDistance = 6f;
    public float repositionTime = 1.5f;

    private Transform player;
    private Rigidbody2D rb;

    private float repositionTimer;
    private Vector2 randomRepositionSpot;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = State.Moving;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                MoveAndCheckLOS();
                break;
            case State.Throwing:
                ThrowPickaxe();
                break;
            case State.WaitingForPickaxe:
                break;
            case State.Repositioning:
                RepositionBehavior();
                break;
        }
    }

    private void MoveAndCheckLOS()
    {
        if (player == null) return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (directionToPlayer.x < 0) transform.localScale = new Vector3(1, 1, 1);
        else if (directionToPlayer.x > 0) transform.localScale = new Vector3(-1, 1, 1);

        // 1. The Long Laser: Can we see the player to throw the pickaxe?
        RaycastHit2D losHit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        if (losHit.collider == null && distanceToPlayer <= visionRange)
        {
            rb.linearVelocity = Vector2.zero;
            currentState = State.Throwing;
        }
        else
        {
           
            Vector2 finalMoveDirection = directionToPlayer;

           
            RaycastHit2D wallInFace = Physics2D.Raycast(transform.position, directionToPlayer, wallAvoidanceDistance, obstacleLayer);

            if (wallInFace.collider != null)
            {
                
                Vector2 wallNormal = wallInFace.normal;
                Vector2 slideDirection = new Vector2(-wallNormal.y, wallNormal.x);

                
                if (Vector2.Dot(slideDirection, directionToPlayer) < 0)
                {
                    slideDirection = -slideDirection;
                }

                finalMoveDirection = slideDirection.normalized;
            }

            rb.linearVelocity = finalMoveDirection * moveSpeed;
        }
    }

    private void ThrowPickaxe()
    {
        if (pickaxePrefab != null)
        {
            GameObject thrownPickaxe = Instantiate(pickaxePrefab, transform.position, Quaternion.identity);
            BoomerangPickaxe pickaxeScript = thrownPickaxe.GetComponent<BoomerangPickaxe>();

            if (pickaxeScript != null)
            {
                Vector2 throwDirection = (player.position - transform.position).normalized;
                Vector2 fixedTargetPos = (Vector2)transform.position + (throwDirection * throwDistance);
                pickaxeScript.Throw(transform, fixedTargetPos);
            }
        }
        currentState = State.WaitingForPickaxe;
    }

    public void CatchPickaxe()
    {
        currentState = State.Repositioning;
        repositionTimer = repositionTime;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        randomRepositionSpot = (Vector2)transform.position + (randomDirection * 4f);
    }

    private void RepositionBehavior()
    {
        repositionTimer -= Time.deltaTime;

        Vector2 moveDir = (randomRepositionSpot - (Vector2)transform.position).normalized;

        // --- NEW: Give the repositioning the same Wall Avoidance logic ---
        RaycastHit2D wallInFace = Physics2D.Raycast(transform.position, moveDir, wallAvoidanceDistance, obstacleLayer);
        if (wallInFace.collider != null)
        {
            Vector2 wallNormal = wallInFace.normal;
            Vector2 slideDirection = new Vector2(-wallNormal.y, wallNormal.x);
            if (Vector2.Dot(slideDirection, moveDir) < 0)
            {
                slideDirection = -slideDirection;
            }
            moveDir = slideDirection.normalized;
        }

        rb.linearVelocity = moveDir * moveSpeed;

        if (moveDir.x < 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveDir.x > 0) transform.localScale = new Vector3(-1, 1, 1);

        if (repositionTimer <= 0)
        {
            currentState = State.Moving;
        }
    }
}