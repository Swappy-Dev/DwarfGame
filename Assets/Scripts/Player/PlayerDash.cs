using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference dashActionReference;
    [SerializeField] private TrailRenderer trail; // 

    private Rigidbody2D rb;
    private PlayerData playerData;
    private Camera mainCam;

    [Header("Dash Settings")]
    [SerializeField] private float dashPower = 25f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private bool canDash = true;
    public bool isDashing { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();
        mainCam = Camera.main;
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

        if (trail != null)
            trail.emitting = true; // START TRAIL

        Vector2 dashDir = GetMouseDirection();

        playerData.SetInvincibility(dashDuration);

        rb.linearVelocity = dashDir * dashPower;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        if (trail != null)
            trail.emitting = false; // STOP TRAIL

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private Vector2 GetMouseDirection()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;

        return (mousePos - transform.position).normalized;
    }
}