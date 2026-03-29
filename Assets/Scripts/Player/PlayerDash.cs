using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference dashActionReference;
    private Rigidbody2D rb;
    private PlayerData playerData;
    private Camera mainCam;

    [Header("Dash Settings")]
    [SerializeField] private float dashPower = 25f; // Made it a bit faster for mouse aiming
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private bool canDash = true;
    public bool isDashing { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();
        mainCam = Camera.main; // Find the camera once at the start
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

        // 1. Calculate direction toward the mouse
        Vector2 dashDir = GetMouseDirection();

        // 2. Trigger I-frames in PlayerData
        playerData.SetInvincibility(dashDuration);

        // 3. Apply the dash
        rb.linearVelocity = dashDir * dashPower;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private Vector2 GetMouseDirection()
    {
        // Get mouse position in world space (similar to your CrosshairScript)
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;

        // Direction = Target Position - Current Position
        Vector2 direction = (mousePos - transform.position).normalized;

        return direction;
    }
}