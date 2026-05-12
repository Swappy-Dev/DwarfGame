using UnityEngine;
using System.Collections;

public class Dynamite : MonoBehaviour
{
    [Header("Dynamite Settings")]
    public float speed = 7f;
    public float fuseTime = 1.5f;
    public float explosionRadius = 2.5f;
    public int damage = 1;

    [Header("Visuals")]
    public GameObject explosionParticles;
    public Transform warningCircle;

    private Vector2 targetPosition;
    private bool isAtTarget = false;

    public void Throw(Vector2 targetPos)
    {
        targetPosition = targetPos;
        isAtTarget = false;

        // Hide the warning circle while flying
        if (warningCircle != null)
        {
            warningCircle.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isAtTarget)
        {
            // Fly to target
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            transform.Rotate(0, 0, 360 * Time.deltaTime);

            // Did we land?
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                isAtTarget = true;

                if (warningCircle != null)
                {
                    warningCircle.gameObject.SetActive(true);

                    // Detach from dynamite so it stays flat on the floor
                    warningCircle.SetParent(null);
                    warningCircle.rotation = Quaternion.identity;

                    // Start it at size ZERO!
                    warningCircle.localScale = Vector3.zero;
                }

                StartCoroutine(FuseRoutine());
            }
        }
    }

    private IEnumerator FuseRoutine()
    {
        float timer = 0f;
        Vector3 startingScale = Vector3.zero;

        // The final size we want the circle to be
        Vector3 finalScale = new Vector3(explosionRadius * 2f, explosionRadius * 2f, 1f);

        // This loop makes it smoothly grow larger every single frame!
        while (timer < fuseTime)
        {
            timer += Time.deltaTime;

            // Calculate a percentage (0.0 to 1.0) of how close it is to exploding
            float progress = timer / fuseTime;

            if (warningCircle != null)
            {
                // Lerp smoothly blends between the start size and final size based on the timer
                warningCircle.localScale = Vector3.Lerp(startingScale, finalScale, progress);
            }

            // Wait for the next frame before looping again
            yield return null;
        }

        // Timer is up, BOOM!
        Explode();
    }

    private void Explode()
    {
        Collider2D[] caughtInBlast = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        bool hasDamagedPlayer = false;

        foreach (Collider2D hit in caughtInBlast)
        {
            if (hit.CompareTag("Player") && hasDamagedPlayer == false)
            {
                PlayerData playerStats = hit.GetComponent<PlayerData>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(damage);
                    hasDamagedPlayer = true;
                }
            }
        }

        if (explosionParticles != null)
        {
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
        }

        // Clean up the detached circle
        if (warningCircle != null)
        {
            Destroy(warningCircle.gameObject);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}