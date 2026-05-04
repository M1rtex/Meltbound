using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class SimpleTeleport : MonoBehaviour
{
    [SerializeField] private Transform destination;
    [SerializeField] private float travelDuration = 0.5f;
    [SerializeField] private Key activationKey = Key.S;

    private bool isPlayerInPipe = false;
    private GameObject player;
    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInPipe = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInPipe = false;
            player = null;
        }
    }

    private void Update()
    {
        if (isPlayerInPipe && !isTeleporting && Keyboard.current[activationKey].wasPressedThisFrame)
        {
            StartCoroutine(SmoothTravelRoutine());
        }
    }

    IEnumerator SmoothTravelRoutine()
    {

        if (destination == null)
        {
            Debug.LogError($"На объекте {gameObject.name} не назначена точка Destination!");
            yield break; // Выходим из корутины, чтобы не было ошибки
        }
        
        if (player == null)
        {
            Debug.LogWarning("Попытка телепортации, но объект игрока не найден.");
            yield break;
        }

        isTeleporting = true;

        Vector3 startPos = player.transform.position;
        Vector3 targetPos = destination.position;

        // 1. Подготовка компонентов
        var activePlayer = player;
        var rb = activePlayer.GetComponent<Rigidbody2D>();
        var movement = activePlayer.GetComponent<PlayerMovement>();
        var sprite = activePlayer.GetComponent<SpriteRenderer>();

        // Выключаем управление и физику (чтобы не падал во время полета)
        if (movement != null) movement.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        
        float elapsedTime = 0;

        // 2. Процесс плавного перемещения (Lerp)
        while (elapsedTime < travelDuration)
        {   
            // Проверяем, не удалили ли игрока случайно во время полета
            if (activePlayer == null) {
                yield break;
            }
            
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / travelDuration;

            // Плавное движение по прямой
            activePlayer.transform.position = Vector3.Lerp(startPos, targetPos, t);

            // Плейсхолдер анимации: крутим снеговика вокруг своей оси
            activePlayer.transform.Rotate(0, 0, 720 * Time.deltaTime);

            yield return null; // Ждем следующего кадра
        }

        // 3. Завершение
        if (activePlayer != null)
        {
            activePlayer.transform.position = targetPos;
            activePlayer.transform.rotation = Quaternion.identity; // Сбрасываем вращение
            rb.bodyType = RigidbodyType2D.Dynamic;
            if (movement != null) movement.enabled = true;
        }

        isTeleporting = false;
    }
}