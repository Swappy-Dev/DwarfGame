using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference dashActionReference; [SerializeField] private TrailRenderer trail;
    private Rigidbody2D rb;
    private PlayerData playerData;
    private Camera mainCam;
    private SpriteRenderer spriteRenderer;
    [Header("Dash Settings")]
    [SerializeField] private float dashPower = 25f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [Header("Afterimage Settings")]
    [SerializeField] private float afterimageInterval = 0.05f;
    [SerializeField] private float afterimageFadeDuration = 0.2f;
    [SerializeField] private Color afterimageColor = new Color(1f, 1f, 1f, 0.5f);
    private bool canDash = true;
    public bool isDashing { get; private set; }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();
        mainCam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Make trail semi-transparent
        if (trail != null)
        {
            trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 0.4f);
            trail.endColor = new Color(trail.endColor.r, trail.endColor.g, trail.endColor.b, 0f);
        }
    }
    private void OnEnable()
    {
        dashActionReference.action.Enable();
        dashActionReference.action.performed += OnDashPerformed;
    }
    private void OnDisable()
    {
        dashActionReference.action.performed -= OnDashPerformed;
        dashActionReference.action.Disable();
    }
    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }
    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;
        SoundManager.Instance.Play("Dash");
        if (trail != null)
            trail.emitting = true;
        Vector2 dashDir = GetMouseDirection();
        playerData.SetInvincibility(dashDuration);
        rb.linearVelocity = dashDir * dashPower;
        StartCoroutine(SpawnAfterimages());
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
        if (trail != null)
            trail.emitting = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    private IEnumerator SpawnAfterimages()
    {
        while (isDashing)
        {
            SpawnAfterimage();
            yield return new WaitForSeconds(afterimageInterval);
        }
    }
    private void SpawnAfterimage()
    {
        if (spriteRenderer == null) return;
        // Create a new GameObject at the player's current position and scale
        GameObject ghost = new GameObject("Afterimage");
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.localScale;
        // Copy the current sprite
        SpriteRenderer ghostRenderer = ghost.AddComponent<SpriteRenderer>();
        ghostRenderer.sprite = spriteRenderer.sprite;
        ghostRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        ghostRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        ghostRenderer.color = afterimageColor;
        // Fade it out and destroy it
        StartCoroutine(FadeAfterimage(ghostRenderer));
    }
    private IEnumerator FadeAfterimage(SpriteRenderer ghostRenderer)
    {
        float elapsed = 0f;
        Color startColor = ghostRenderer.color;
        while (elapsed < afterimageFadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / afterimageFadeDuration);
            ghostRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        Destroy(ghostRenderer.gameObject);
    }
    private Vector2 GetMouseDirection()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;
        return (mousePos - transform.position).normalized;
    }
}