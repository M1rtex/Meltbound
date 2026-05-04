using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f; // Поставил 5f, так как 0.01f обычно слишком мало
    [SerializeField] private float jumpForce = 10f;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpPower = new Vector2(5f, 10f); // Сила отскока (X - от стены, Y - вверх)
    [SerializeField] private float wallJumpDuration = 0.2f; // Время блокировки управления

    [Header("Detection Settings")]
    [SerializeField] private LayerMask environmentLayer; // ЕДИНЫЙ слой для земли и стен
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private float horizontalInput;
    private float wallJumpTimer; // Замена корутине
    private bool isFacingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Таймер блокировки управления после отскока от стены
        if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
        }
        else
        {
            // Поворот персонажа только если мы контролируем его (не в процессе отскока)
            if (horizontalInput > 0 && !isFacingRight) Flip();
            else if (horizontalInput < 0 && isFacingRight) Flip();
        }

        animator.SetBool("PlayerRun", horizontalInput != 0);
    }

    private void FixedUpdate()
    {
        // Движемся только если управление не заблокировано прыжком от стены
        if (wallJumpTimer <= 0)
        {
            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    // --- INPUT SYSTEM METHODS ---

    public void OnMove(InputValue value)
    {
        horizontalInput = value.Get<Vector2>().x;
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;

        if (IsGrounded())
        {
            // Обычный прыжок
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else if (IsWalled())
        {
            // Прыжок от стены
            WallJump();
        }
    }

    // --- MECHANICS ---

    private void WallJump()
    {
        // Включаем таймер блокировки управления
        wallJumpTimer = wallJumpDuration;
        
        // Так как wallCheck находится перед лицом, если мы смотрим вправо (true),
        // значит стена справа, и прыгать надо влево (-1). И наоборот.
        float wallDirection = isFacingRight ? -1f : 1f;
        
        // Сбрасываем текущую скорость и применяем силу отскока
        rb.linearVelocity = new Vector2(wallDirection * wallJumpPower.x, wallJumpPower.y);
        
        // Сразу разворачиваем персонажа лицом от стены
        Flip();
    }

    // --- DETECTIONS ---

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, environmentLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, environmentLayer);
    }

    // --- GIZMOS ---

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (groundCheck) Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        
        Gizmos.color = Color.red;
        if (wallCheck) Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
    }
}