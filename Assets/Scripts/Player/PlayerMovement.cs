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

    // The timer that controls how long we are stunned
    public float knockbackTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
       
        if (knockbackTimer > 0)
        {
            
            knockbackTimer -= Time.fixedDeltaTime;

            // Exit immediately! Do NOT run the walking code below.
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

    // Del movement directions, netrinti fu fu fu

    [SerializeField] private Animator animator;

    private void Update()
    {
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        // 1. Reset all bools to false first so they don't get "stuck" on
        animator.SetBool("IsWalkingRight", false);
        animator.SetBool("IsWalkingLeft", false);
        animator.SetBool("IsWalkingUp", false);
        animator.SetBool("IsWalkingDown", false);

        // 2. Check direction and set the correct bool
        // We use a small threshold (0.1) to avoid jitter
        if (movementInput.x > 0.1f)
        {
            animator.SetBool("IsWalkingRight", true);
        }
        else if (movementInput.x < -0.1f)
        {
            animator.SetBool("IsWalkingLeft", true);
        }
        else if (movementInput.y > 0.1f)
        {
            animator.SetBool("IsWalkingUp", true);
        }
        else if (movementInput.y < -0.1f)
        {
            animator.SetBool("IsWalkingDown", true);
        }
    }
}