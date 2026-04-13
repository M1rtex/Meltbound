using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 0.01f;
    [SerializeField] private float jumpForce = 0.01f;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpPower = new Vector2(2f, 6f); // Сила отскока (X - в сторону, Y - вверх)
    [SerializeField] private float wallJumpDuration = 0.1f; // Время блокировки управления

    [Header("Detection Settings")]
    [SerializeField] private Transform groundCheck; // Пустой объект в ногах игрока
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 checkSize = new Vector2(0.3f, 0.1f);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask WallLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private float horizontalInput;
    private bool isWallJumping;
    private bool isFacingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Проверяем ввод и текущее направление
        if (!isWallJumping) 
        {
            if (horizontalInput > 0 && !isFacingRight) Flip();
            else if (horizontalInput < 0 && isFacingRight) Flip();
        }

        animator.SetBool("PlayerRun", horizontalInput != 0);
    }

    private void Flip()
    {
        // Меняем логическое состояние
        isFacingRight = !isFacingRight;

        // Получаем текущий масштаб и инвертируем X
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    // Метод для движения (WASD)
    public void OnMove(InputValue value)
    {
        horizontalInput = value.Get<Vector2>().x;
    }

    // Метод для прыжка (Space)
    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;

        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else if (IsWalled())
        {
            StartCoroutine(WallJump());
        }
    }
    
    private IEnumerator WallJump()
    {
        isWallJumping = true;
        
        // Определяем направление отскока: если стена справа, прыгаем влево (-1), и наоборот
        float wallDirection = IsWalledRight() ? -1f : 1f;
        
        // Применяем силу отскока
        rb.linearVelocity = new Vector2(wallDirection * wallJumpPower.x, wallJumpPower.y);
        
        // Поворачиваем персонажа в сторону прыжка (опционально)
        Flip();

        yield return new WaitForSeconds(wallJumpDuration);
        
        isWallJumping = false;
    }
    
    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        }
    }

    private bool IsGrounded()
    {
        // Создаем невидимую зону в ногах для проверки слоя Ground
        return Physics2D.OverlapBox(groundCheck.position, checkSize, 0f, groundLayer) || 
            Physics2D.OverlapBox(groundCheck.position, checkSize, 0f, WallLayer);
    }

    private bool IsWalled() => Physics2D.OverlapCircle(wallCheck.position, 0.2f, WallLayer);

    private bool IsWalledRight()
    {
        // Проверяем наличие коллайдера чуть правее центра игрока
        return Physics2D.Raycast(transform.position, Vector2.right, 0.6f, WallLayer);
    }

    // Отрисовка зоны проверки в редакторе для удобства
    private void OnDrawGizmos()
    {
        if (groundCheck) Gizmos.DrawWireCube(groundCheck.position, checkSize);
        if (wallCheck) Gizmos.DrawWireSphere(wallCheck.position, 0.2f);
    }
}