using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference dashActionReference;
    [SerializeField] private TrailRenderer trail;

    private Rigidbody2D rb;
    private PlayerData playerData;
    private SpriteRenderer spriteRenderer;

    [Header("Dash Settings")]
    [SerializeField] private float dashPower = 25f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Afterimage Settings")]
    [SerializeField] private float afterimageInterval = 0.05f;
    [SerializeField] private float afterimageFadeDuration = 0.5f;
    [SerializeField] private Color afterimageColor = new Color(1f, 1f, 1f, 0.5f);

    private bool canDash = true;
    public bool isDashing { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (trail != null) trail.emitting = false;
    }

    private void OnEnable()
    {
        if (dashActionReference != null)
        {
            dashActionReference.action.Enable();
            dashActionReference.action.performed += OnDashPerformed;
        }
    }

    private void OnDisable()
    {
        if (dashActionReference != null)
        {
            dashActionReference.action.performed -= OnDashPerformed;
            dashActionReference.action.Disable();
        }
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (canDash && !isDashing) StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        if (SoundManager.Instance != null) SoundManager.Instance.Play("Dash");
        if (trail != null) trail.emitting = true;

        Vector2 dashDir = GetMouseDirection();
        if (playerData != null) playerData.SetInvincibility(dashDuration);

        rb.linearVelocity = dashDir * dashPower;
        StartCoroutine(SpawnAfterimages());

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.linearVelocity = Vector2.zero;
        if (trail != null) trail.emitting = false;

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

        GameObject ghost = new GameObject("Afterimage");
        ghost.transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.localScale;

        // AfterimageGhost is self-contained — it owns its own SpriteRenderer
        // and fades itself independently of the player object's lifecycle.
        AfterimageGhost fader = ghost.AddComponent<AfterimageGhost>();
        fader.Init(spriteRenderer, afterimageColor, afterimageFadeDuration);
    }

    private Vector2 GetMouseDirection()
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector2.right;

        Vector3 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;
        Vector2 direction = (mousePos - transform.position).normalized;
        return direction == Vector2.zero ? Vector2.right : direction;
    }
}