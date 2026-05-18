using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float minJumpForce = 5f;
    [SerializeField] private float jumpReleaseMultiplier = 0.5f;

    [Header("Wall Jump Settings")]
    [SerializeField] private Vector2 wallJumpPower = new Vector2(5f, 10f);
    [SerializeField] private float wallJumpDuration = 0.2f;

    [Header("Input Buffer Settings")]
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.15f;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask environmentLayer;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);

    [Header("Wall Check (Кубы)")][Tooltip("Объект wallCheck должен быть РОВНО В ЦЕНТРЕ персонажа")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.15f, 0.5f);
    [SerializeField] private float wallCheckOffset = 0.3f; // На сколько кубы сдвинуты влево и вправо от центра

    [Header("Slope Settings")]
    [SerializeField] private float maxSlopeAngle = 45f; // Максимальный угол наклона, по которому можно ходить
    [SerializeField] private float slopeCheckDistance = 0.5f; // Дистанция проверки наклона

    private Rigidbody2D rb;
    private Animator animator;
    private float horizontalInput;
    private float wallJumpTimer;
    private bool isFacingRight = true;

    private float jumpBufferCounter;
    private bool wasGroundedLastFrame;
    private float coyoteTimeCounter;
    private bool isJumping;
    private bool isJumpButtonHeld;
    private PlayerInput playerInput;
    private InputAction jumpAction;

    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private bool isOnSlope;
    private bool canWalkOnSlope;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions["Jump"];
    }

    private void Update()
    {
        bool isGrounded = IsGrounded();

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (isJumpButtonHeld && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (1f - jumpReleaseMultiplier) * Time.deltaTime;
        }

        if (jumpAction.WasReleasedThisFrame() && rb.linearVelocity.y > 0)
        {
            isJumpButtonHeld = false;
            float newVelocityY = rb.linearVelocity.y * jumpReleaseMultiplier;
            newVelocityY = Mathf.Max(newVelocityY, minJumpForce);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, newVelocityY);
        }

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;

            if (isGrounded && !wasGroundedLastFrame)
            {
                PerformJump();
                jumpBufferCounter = 0;
            }
        }

        wasGroundedLastFrame = isGrounded;

        if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
        }
        else
        {
            if (horizontalInput > 0 && !isFacingRight) Flip();
            else if (horizontalInput < 0 && isFacingRight) Flip();
        }

        if (animator != null)
            animator.SetBool("PlayerRun", horizontalInput != 0);
            animator.SetFloat("HorizontalSpeed", horizontalInput);
    }

    private void FixedUpdate()
    {
        CheckSlope();

        if (wallJumpTimer <= 0)
        {
            float targetSpeed = horizontalInput * speed;

            float currentAcceleration = 50f;

            if (IsGrounded())
            {
                RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, environmentLayer);

                if (hit.collider != null)
                {
                    float surfaceFriction = hit.collider.friction;

                    if (surfaceFriction < 0.1f)
                        currentAcceleration = 5f; // Эффект льда
                    else if (surfaceFriction >= 1f)
                        currentAcceleration = 100f; // Эффект грязи (мгновенный стоп)
                    else
                        currentAcceleration = 40f; // Обычный пол
                }

                if (isOnSlope && canWalkOnSlope)
                {
                    float moveDirectionX = horizontalInput * speed;
                    Vector2 moveDirection = new Vector2(slopeNormalPerp.x * -moveDirectionX, slopeNormalPerp.y * -moveDirectionX);

                    float newX = Mathf.MoveTowards(rb.linearVelocity.x, moveDirection.x, currentAcceleration * Time.fixedDeltaTime);
                    float newY = Mathf.MoveTowards(rb.linearVelocity.y, moveDirection.y, currentAcceleration * Time.fixedDeltaTime);

                    rb.linearVelocity = new Vector2(newX, newY);
                    return;
                }
            }
            else
            {
                currentAcceleration = 15f;
            }

            if (horizontalInput == 0 && currentAcceleration < 10f)
                currentAcceleration = 2f;

            float newVelX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, currentAcceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newVelX, rb.linearVelocity.y);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
    }

    public void OnMove(InputValue value)
    {
        horizontalInput = value.Get<Vector2>().x;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            isJumpButtonHeld = true;
            jumpBufferCounter = jumpBufferTime;

            if (coyoteTimeCounter > 0)
            {
                PerformJump();
                coyoteTimeCounter = 0;
            }
            else if (IsWalledLeft() || IsWalledRight())
            {
                WallJump();
            }
        }
    }

    private void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;
    }

    private void WallJump()
    {
        wallJumpTimer = wallJumpDuration;
        
        float wallDirection = IsWalledLeft() ? 1f : -1f;
        
        rb.linearVelocity = new Vector2(wallDirection * wallJumpPower.x, wallJumpPower.y);
        
        if (wallDirection > 0 && !isFacingRight) Flip();
        else if (wallDirection < 0 && isFacingRight) Flip();
    }

    // --- ПРОВЕРКИ ---

    public bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, environmentLayer);
    }

    private void CheckSlope()
    {
        Vector2 checkPos = groundCheck.position;

        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, environmentLayer);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != 0)
            {
                isOnSlope = true;
            }
            else
            {
                isOnSlope = false;
            }

            canWalkOnSlope = slopeDownAngle <= maxSlopeAngle;
        }
        else
        {
            isOnSlope = false;
            canWalkOnSlope = false;
        }
    }

    private bool IsWalledLeft()
    {
        Vector2 leftPosition = (Vector2)wallCheck.position + (Vector2.left * wallCheckOffset);
        return Physics2D.OverlapBox(leftPosition, wallCheckSize, 0f, environmentLayer);
    }

    private bool IsWalledRight()
    {
        Vector2 rightPosition = (Vector2)wallCheck.position + (Vector2.right * wallCheckOffset);
        return Physics2D.OverlapBox(rightPosition, wallCheckSize, 0f, environmentLayer);
    }

    // --- ОТРИСОВКА В РЕДАКТОРЕ (GIZMOS) ---

    private void OnDrawGizmosSelected()
    {
        // Рисуем землю
        Gizmos.color = Color.green;
        if (groundCheck) Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

        // Рисуем стены
        Gizmos.color = Color.red;
        if (wallCheck)
        {
            // Левый куб
            Vector2 leftPos = (Vector2)wallCheck.position + (Vector2.left * wallCheckOffset);
            Gizmos.DrawWireCube(leftPos, wallCheckSize);

            // Правый куб
            Vector2 rightPos = (Vector2)wallCheck.position + (Vector2.right * wallCheckOffset);
            Gizmos.DrawWireCube(rightPos, wallCheckSize);
        }

        // Рисуем проверку наклона
        if (groundCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(groundCheck.position, (Vector2)groundCheck.position + Vector2.down * slopeCheckDistance);
        }
    }
}