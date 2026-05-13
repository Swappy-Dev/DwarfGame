using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 facingDirection = Vector2.right;
    public float knockbackTimer = 0f;
    public bool isDead = false;
    private PlayerDash playerDash;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerDash = GetComponent<PlayerDash>();
    }
    public Vector2 GetMovementInput()
    {
        return movementInput;
    }
    public void SetFacingDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
            facingDirection = dir.normalized;
    }
    private float footstepTimer = 0f;
    public float footstepInterval = 0.35f;
    private void FixedUpdate()
    {
        if (isDead) return;
        if (playerDash != null && playerDash.isDashing) return;
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }
        Vector2 movement = movementInput.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
        if (movementInput != Vector2.zero)
        {
            footstepTimer -= Time.fixedDeltaTime;
            if (footstepTimer <= 0f)
            {
                SoundManager.Instance.Play("Walking");
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
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
    private void Update()
    {
        if (isDead) return;
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
        bool isMoving = movementInput.sqrMagnitude > 0.01f;
        Vector2 directionToUse = isMoving ? movementInput : facingDirection;
        if (Mathf.Abs(directionToUse.x) >= Mathf.Abs(directionToUse.y))
        {
            if (directionToUse.x > 0) animator.SetBool("IsWalkingRight", true);
            else animator.SetBool("IsWalkingLeft", true);
        }
        else
        {
            if (directionToUse.y > 0) animator.SetBool("IsWalkingUp", true);
            else animator.SetBool("IsWalkingDown", true);
        }
    }
}