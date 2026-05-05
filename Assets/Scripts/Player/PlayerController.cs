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
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.15f, 0.5f); // Размер кубов (ширина, высота)
    [SerializeField] private float wallCheckOffset = 0.3f; // На сколько кубы сдвинуты влево и вправо от центра

    private Rigidbody2D rb;
    private Animator animator;
    private float horizontalInput;
    private float wallJumpTimer;
    private bool isFacingRight = true;

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
        if (wallJumpTimer <= 0)
        {
            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
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

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, environmentLayer);
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
    }
}