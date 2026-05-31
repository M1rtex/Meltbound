using UnityEngine;
using System.Collections;

/// <summary>
/// Автоматическая телепортация через трубы с эффектом выталкивания игрока.
/// При касании триггера игрок мгновенно перемещается к destination,
/// камера плавно летит к новой позиции, затем игрока выталкивает в заданном направлении.
/// </summary>
public class SimpleTeleport : MonoBehaviour
{
    [Header("Настройки телепортации")]
    [SerializeField] private Transform destination;
    [Tooltip("Время, пока камера летит к новой точке (секунды)")]
    [SerializeField] private float travelTime = 0.5f;

    [Header("Настройки выталкивания")]
    [Tooltip("Направление выталкивания игрока (например, (0, 1) — вверх, (1, 0) — вправо)")]
    [SerializeField] private Vector2 launchDirection = Vector2.up;
    [Tooltip("Сила выталкивания снеговика")]
    [SerializeField] private float launchForce = 10f;

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что вошел игрок и телепортация не активна
        if (collision.CompareTag("Player") && !isTeleporting)
        {
            Debug.Log($"[SimpleTeleport] Игрок вошел в триггер на {gameObject.name}. Начинаем телепортацию.");
            StartCoroutine(TeleportRoutine(collision.gameObject));
        }
    }

    /// <summary>
    /// Корутина телепортации: отключает управление, плавно перемещает игрока с вращением, выталкивает.
    /// </summary>
    private IEnumerator TeleportRoutine(GameObject player)
    {
        // Проверка наличия точки назначения
        if (destination == null)
        {
            Debug.LogError($"[SimpleTeleport] На объекте {gameObject.name} не назначена точка Destination!");
            yield break;
        }

        if (player == null)
        {
            Debug.LogWarning("[SimpleTeleport] Объект игрока не найден.");
            yield break;
        }

        // Устанавливаем флаг телепортации
        isTeleporting = true;

        // Получаем компоненты игрока
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        Collider2D playerCollider = player.GetComponent<Collider2D>();

        if (rb == null)
        {
            Debug.LogError("[SimpleTeleport] У игрока отсутствует Rigidbody2D!");
            isTeleporting = false;
            yield break;
        }

        Debug.Log("[SimpleTeleport] Отключаем управление и коллайдер игрока.");

        // 1. ОТКЛЮЧАЕМ управление и коллайдер перед телепортацией
        if (movement != null) movement.enabled = false;
        if (playerCollider != null) playerCollider.enabled = false;

        // Переводим в кинематический режим и обнуляем скорость
        rb.isKinematic = true;
        rb.linearVelocity = Vector2.zero;

        // Сохраняем исходный масштаб (если захотим уменьшить снеговика в полёте)
        Vector3 originalScale = player.transform.localScale;

        // Опционально: уменьшаем масштаб, чтобы казалось, что протискивается в трубу
        // player.transform.localScale *= 0.7f;

        // 2. ПЛАВНОЕ ПЕРЕМЕЩЕНИЕ с вращением
        Vector3 startPosition = player.transform.position;
        float elapsed = 0f;

        Debug.Log($"[SimpleTeleport] Начинаем плавный полёт от {startPosition} к {destination.position}");

        while (elapsed < travelTime)
        {
            // Проверяем, не удалили ли игрока во время полёта
            if (player == null)
            {
                Debug.LogWarning("[SimpleTeleport] Игрок был удалён во время телепортации!");
                isTeleporting = false;
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / travelTime;

            // Плавно перемещаем между трубами
            player.transform.position = Vector3.Lerp(startPosition, destination.position, t);

            // Красиво крутим снеговика в полёте вокруг своей оси (720 градусов в секунду)
            player.transform.Rotate(0, 0, 720f * Time.deltaTime);

            yield return null; // Ждём следующего кадра
        }

        // 3. ФИНАЛ ПОЛЁТА - убеждаемся, что игрок точно в конечной точке
        player.transform.position = destination.position;

        // Сбрасываем поворот игрока в дефолтный (чтобы не стоял вверх ногами)
        player.transform.rotation = Quaternion.identity;

        // Возвращаем исходный масштаб, если меняли
        player.transform.localScale = originalScale;

        Debug.Log("[SimpleTeleport] Полёт завершён. Включаем игрока обратно.");

        // 4. ВКЛЮЧАЕМ игрока обратно
        rb.isKinematic = false;

        if (playerCollider != null) playerCollider.enabled = true;
        if (movement != null) movement.enabled = true;

        // 5. ВЫТАЛКИВАЕМ игрока в заданном направлении
        Vector2 normalizedDirection = launchDirection.normalized;
        Vector2 force = normalizedDirection * launchForce;

        rb.AddForce(force, ForceMode2D.Impulse);

        Debug.Log($"[SimpleTeleport] Игрок вытолкнут! Направление: {normalizedDirection}, Сила: {launchForce}, Итоговый импульс: {force}");

        // Сбрасываем флаг телепортации
        isTeleporting = false;

        Debug.Log("[SimpleTeleport] Телепортация завершена.");
    }

    // Визуализация направления выталкивания в редакторе
    private void OnDrawGizmos()
    {
        if (destination != null)
        {
            // Рисуем линию от этого объекта к точке назначения
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destination.position);

            // Рисуем стрелку направления выталкивания в точке назначения
            Gizmos.color = Color.red;
            Vector3 launchStart = destination.position;
            Vector3 launchEnd = launchStart + (Vector3)(launchDirection.normalized * 2f);
            Gizmos.DrawLine(launchStart, launchEnd);

            // Рисуем конус стрелки
            Vector3 arrowTip = launchEnd;
            Vector3 perpendicular = Vector3.Cross(launchDirection.normalized, Vector3.forward).normalized * 0.3f;
            Gizmos.DrawLine(arrowTip, arrowTip - (Vector3)(launchDirection.normalized * 0.5f) + perpendicular);
            Gizmos.DrawLine(arrowTip, arrowTip - (Vector3)(launchDirection.normalized * 0.5f) - perpendicular);
        }
    }
}
