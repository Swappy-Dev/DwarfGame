using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private InputActionReference moveActionReference;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    public float knockbackTimer = 0f;

    public bool isDead = false; // ADD this field

    // --- NEW: Reference to the Dash script ---
    private PlayerDash playerDash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // --- NEW: Get the dash component ---
        playerDash = GetComponent<PlayerDash>();
    }

    // --- NEW: Public function so PlayerDash can see which way we are moving ---
    public Vector2 GetMovementInput()
    {
        return movementInput;
    }

    private void FixedUpdate()
    {

        if (isDead) return; // ADD this
        if (playerDash != null && playerDash.isDashing) return;

        // --- NEW: Prevent walking if we are currently dashing ---
        if (playerDash != null && playerDash.isDashing) return;

        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector2 movement = movementInput.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    void OnEnable()
    {
        moveActionReference.action.Enable();
        moveActionReference.action.performed += OnMovedPerformed;
        moveActionReference.action.canceled += OnMoveCanceled;
    }

    void OnDisable()
    {
        moveActionReference.action.performed -= OnMovedPerformed;
        moveActionReference.action.canceled -= OnMoveCanceled;
        moveActionReference.action.Disable();
    }

    private void OnMovedPerformed(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
    }

    [SerializeField] private Animator animator;

    private void Update()
    {

        if (isDead) return; // ADD this
        if (playerDash != null && playerDash.isDashing) return;
        UpdateAnimations();


        // --- NEW: Don't update "Walking" animations if we are dashing ---
        if (playerDash != null && playerDash.isDashing) return;

        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetBool("IsWalkingRight", false);
        animator.SetBool("IsWalkingLeft", false);
        animator.SetBool("IsWalkingUp", false);
        animator.SetBool("IsWalkingDown", false);

        if (movementInput.x > 0.1f) animator.SetBool("IsWalkingRight", true);
        else if (movementInput.x < -0.1f) animator.SetBool("IsWalkingLeft", true);
        else if (movementInput.y > 0.1f) animator.SetBool("IsWalkingUp", true);
        else if (movementInput.y < -0.1f) animator.SetBool("IsWalkingDown", true);
    }
}