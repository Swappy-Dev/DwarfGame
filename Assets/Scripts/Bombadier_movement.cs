using UnityEngine;
using System.Collections;

public class Bombadier_movement : MonoBehaviour
{
    [Header("Animation Timings")]
    public float digDownDuration = 0.55f;
    public float undergroundDuration = 1.5f;
    public float digUpDuration = 1.2f;
    public float timeBetweenDigs = 3f;

    [Header("Combat & Placement")]
    public float popupDistance = 3f;
    public GameObject dynamitePrefab; // --- NEW: Slot for the dynamite! ---

    private Transform player;
    private Animator animator;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        StartCoroutine(DigRoutine());
    }

    void Update()
    {
        if (player != null)
        {
            Vector2 directionToPlayer = player.position - transform.position;

            if (directionToPlayer.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (directionToPlayer.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    private IEnumerator DigRoutine()
    {
        while (true)
        {
            // 1. IDLE
            animator.Play("Idle"); // Make sure this matches your Animator exactly!
            yield return new WaitForSeconds(timeBetweenDigs);

            // 2. DIG DOWN
            col.enabled = false;
            animator.Play("Bombadier_Dig_Down");
            yield return new WaitForSeconds(digDownDuration);

            // 3. UNDERGROUND
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(undergroundDuration);

            // 4. TELEPORT TO PLAYER
            if (player != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                transform.position = (Vector2)player.position + (randomDirection * popupDistance);
            }

            // 5. DIG UP
            spriteRenderer.enabled = true;
            animator.Play("Bombadier_Dig_Up");
            yield return new WaitForSeconds(digUpDuration);

            // 6. --- NEW: THROW THE DYNAMITE ---
            if (dynamitePrefab != null && player != null)
            {
                GameObject thrownDynamite = Instantiate(dynamitePrefab, transform.position, Quaternion.identity);
                Dynamite dynamiteScript = thrownDynamite.GetComponent<Dynamite>();

                if (dynamiteScript != null)
                {
                    // Throw it exactly where the player is standing right now!
                    dynamiteScript.Throw(player.position);
                }
            }

            // 7. READY TO BE HIT AGAIN
            col.enabled = true;
        }
    }
}