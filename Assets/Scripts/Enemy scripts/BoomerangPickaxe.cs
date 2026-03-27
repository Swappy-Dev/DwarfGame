using UnityEngine;

public class BoomerangPickaxe : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;

    private Transform mole;
    private MinerMole moleScript; // Updated from MoleAI to MinerMole
    private Vector2 targetPosition;
    private bool isReturning = false;
    private bool hasHitPlayer = false;

    public void Throw(Transform thrower, Vector2 targetPos)
    {
        mole = thrower;
        // Make sure this matches your actual script name
        moleScript = thrower.GetComponent<MinerMole>();
        targetPosition = targetPos;
        isReturning = false;
    }

    void Update()
    {
        // 1. Safety Check: If the mole was destroyed, just destroy the pickaxe too
        if (mole == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.Rotate(0, 0, -800 * Time.deltaTime);

        if (!isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                isReturning = true;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, mole.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, mole.position) < 0.5f)
            {
                // 2. Safety Check: Only call CatchPickaxe if we successfully found the script
                if (moleScript != null)
                {
                    moleScript.CatchPickaxe();
                }

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && !hasHitPlayer)
        {
            hasHitPlayer = true;

            PlayerData playerStats = collider.GetComponent<PlayerData>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
            isReturning = true;
        }

        // Check for walls/obstacles
        if (collider.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            isReturning = true;
        }
    }
}