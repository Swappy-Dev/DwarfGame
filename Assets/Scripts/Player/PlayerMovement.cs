// PlayerMovement.cs
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 facingDirection = Vector2.down;
    public float knockbackTimer = 0f;
    public bool isDead = false;
    private PlayerDash playerDash;

    private bool hasPickaxe = false;
    public bool HasPickaxe => hasPickaxe;

    private bool isAttacking = false;
    public bool IsAttacking => isAttacking;

    private float attackTimeoutTimer = 0f;
    private const float ATTACK_TIMEOUT = 1.5f;

    private float footstepTimer = 0f;
    public float footstepInterval = 0.35f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerDash = GetComponent<PlayerDash>();
    }

    public Vector2 GetMovementInput() => movementInput;
    public bool IsMoving() => movementInput.sqrMagnitude > 0.01f;

    public void SetFacingDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
            facingDirection = dir.normalized;
    }

    public void TriggerAttackAnimation()
{
    if (animator != null)
    {
        isAttacking = true;
        attackTimeoutTimer = ATTACK_TIMEOUT;

        // Clear all direction bools so Any State can't fire walk/idle transitions
        animator.SetBool("IsWalkingRight", false);
        animator.SetBool("IsWalkingLeft", false);
        animator.SetBool("IsWalkingUp", false);
        animator.SetBool("IsWalkingDown", false);
        animator.SetBool("IsIdle", false);

        animator.SetTrigger("Attack");
    }
}

    public void OnAttackEnd()
    {
        isAttacking = false;
        attackTimeoutTimer = 0f;
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

    private void OnMovedPerformed(InputAction.CallbackContext context) =>
        movementInput = context.ReadValue<Vector2>();

    private void OnMoveCanceled(InputAction.CallbackContext context) =>
        movementInput = Vector2.zero;

    private void Update()
    {
        if (isDead) return;
        if (playerDash != null && playerDash.isDashing) return;

        if (isAttacking)
        {
            attackTimeoutTimer -= Time.deltaTime;
            if (attackTimeoutTimer <= 0f)
            {
                isAttacking = false;
                animator.ResetTrigger("Attack");
            }
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            isAttacking = false;
            attackTimeoutTimer = 0f;
            animator.ResetTrigger("Attack");

            hasPickaxe = !hasPickaxe;
            animator.SetBool("HasPickaxe", hasPickaxe);
        }

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        if (playerDash != null && playerDash.isDashing) return;
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        // Full movement allowed even during attack
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

    private void UpdateAnimations()
    {
        if (animator == null) return;
        if (isAttacking) return;

        bool isMoving = movementInput.sqrMagnitude > 0.01f;
        Vector2 directionToUse = isMoving ? movementInput : facingDirection;

        animator.SetBool("IsIdle", !isMoving);

        animator.SetBool("IsWalkingRight", false);
        animator.SetBool("IsWalkingLeft", false);
        animator.SetBool("IsWalkingUp", false);
        animator.SetBool("IsWalkingDown", false);

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