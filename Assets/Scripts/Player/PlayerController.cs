using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 0.01f;
    [SerializeField] private float jumpForce = 0.01f;

    [Header("Detection Settings")]
    [SerializeField] private Transform groundCheck; // Пустой объект в ногах игрока
    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float horizontalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Метод для движения (WASD)
    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        horizontalInput = input.x;
    }

    // Метод для прыжка (Space)
    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded())
        {
            // Применяем импульс вверх
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        // Движение по горизонтали, сохраняя вертикальную скорость (гравитацию)
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
    }

    private bool IsGrounded()
    {
        // Создаем невидимую зону в ногах для проверки слоя Ground
        return Physics2D.OverlapBox(groundCheck.position, checkSize, 0f, groundLayer);
    }

    // Отрисовка зоны проверки в редакторе для удобства
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, checkSize);
        }
    }
}