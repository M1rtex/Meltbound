using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Wall Jump Settings")]
    [SerializeField] private Vector2 wallJumpPower = new Vector2(5f, 10f);
    [SerializeField] private float wallJumpDuration = 0.2f;

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

    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private bool isOnSlope;
    private bool canWalkOnSlope;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
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

                // Движение по наклонной поверхности
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
                currentAcceleration = 15f; // Ускорение в воздухе (инерция прыжка)
            }

            // Если игрок на льду и отпустил кнопки, даем ему катиться еще дольше
            if (horizontalInput == 0 && currentAcceleration < 10f)
                currentAcceleration = 2f;

            float newVelX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, currentAcceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newVelX, rb.linearVelocity.y);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        // Vector3 localScale = transform.localScale;
        // localScale.x *= -1f;
        // transform.localScale = localScale;
    }

    public void OnMove(InputValue value)
    {
        horizontalInput = value.Get<Vector2>().x;
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;

        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else if (IsWalledLeft() || IsWalledRight())
        {
            WallJump();
        }
    }

    private void WallJump()
    {
        wallJumpTimer = wallJumpDuration;
        
        // Если стена слева, то толкаем вправо (1). Иначе толкаем влево (-1).
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
        // Создаем координату для левого куба
        Vector2 leftPosition = (Vector2)wallCheck.position + (Vector2.left * wallCheckOffset);
        return Physics2D.OverlapBox(leftPosition, wallCheckSize, 0f, environmentLayer);
    }

    private bool IsWalledRight()
    {
        // Создаем координату для правого куба
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