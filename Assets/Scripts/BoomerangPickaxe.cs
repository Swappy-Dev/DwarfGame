using UnityEngine;

public class BoomerangPickaxe : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 10;

    private Transform mole;
    private MoleAI moleAI;
    private Vector2 targetPosition;
    private bool isReturning = false;

    public void Throw(Transform thrower, Vector2 targetPos)
    {
        mole = thrower;
        moleAI = thrower.GetComponent<MoleAI>();
        targetPosition = targetPos;
        isReturning = false;
    }

    void Update()
    {
        if (mole == null) return;

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
                moleAI.CatchPickaxe();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            PlayerData playerStats = collider.GetComponent<PlayerData>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
            isReturning = true;
        }

        // --- NEW: Bounce off walls ---
        if (collider.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            isReturning = true;
        }
    }
}