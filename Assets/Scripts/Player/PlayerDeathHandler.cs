using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Death Screen UI")]
    [SerializeField] private GameObject deathScreenUI; // A Canvas panel you create in the scene

    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void HandleDeath()
    {
        // 1. Lock all movement
        playerMovement.isDead = true;

        // 2. Invert the sprite colour
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(
                1f - spriteRenderer.color.r,
                1f - spriteRenderer.color.g,
                1f - spriteRenderer.color.b
            );
        }

        // Find your camera shake script and stop it
        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        if (cameraShake != null)
        {
            cameraShake.StopAllCoroutines();
            // Also reset the camera position so it doesn't stay offset
            Camera.main.transform.localPosition = Vector3.zero;
        }

        // 3. Show the death/restart screen
        if (deathScreenUI != null)
            deathScreenUI.SetActive(true);

        // 4. Pause the game
        Time.timeScale = 0f;


        Cursor.visible = true;
    }

    // Call this from your "Restart" button in the UI
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        // Unsubscribe static events to prevent them stacking on reload
        PlayerData.ClearEvents();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}