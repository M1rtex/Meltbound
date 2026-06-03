using UnityEngine;
using System.Collections;

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
        if (collision.CompareTag("Player") && !isTeleporting)
        {
            Debug.Log($"[SimpleTeleport] Игрок вошел в триггер на {gameObject.name}. Начинаем телепортацию.");
            StartCoroutine(TeleportRoutine(collision.gameObject));
        }
    }

    private IEnumerator TeleportRoutine(GameObject player)
    {
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

        isTeleporting = true;

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

        if (movement != null) movement.enabled = false;
        if (playerCollider != null) playerCollider.enabled = false;

        rb.isKinematic = true;
        rb.linearVelocity = Vector2.zero;

        Vector3 originalScale = player.transform.localScale;
        Vector3 startPosition = player.transform.position;
        float elapsed = 0f;

        Debug.Log($"[SimpleTeleport] Начинаем плавный полёт от {startPosition} к {destination.position}");

        while (elapsed < travelTime)
        {
            if (player == null)
            {
                Debug.LogWarning("[SimpleTeleport] Игрок был удалён во время телепортации!");
                isTeleporting = false;
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / travelTime;

            player.transform.position = Vector3.Lerp(startPosition, destination.position, t);
            player.transform.Rotate(0, 0, 720f * Time.deltaTime);

            yield return null;
        }

        player.transform.position = destination.position;
        player.transform.rotation = Quaternion.identity;
        player.transform.localScale = originalScale;

        Debug.Log("[SimpleTeleport] Полёт завершён. Включаем игрока обратно.");

        rb.isKinematic = false;

        if (playerCollider != null) playerCollider.enabled = true;
        if (movement != null) movement.enabled = true;

        Vector2 normalizedDirection = launchDirection.normalized;
        Vector2 force = normalizedDirection * launchForce;

        rb.AddForce(force, ForceMode2D.Impulse);

        Debug.Log($"[SimpleTeleport] Игрок вытолкнут! Направление: {normalizedDirection}, Сила: {launchForce}, Итоговый импульс: {force}");

        isTeleporting = false;

        Debug.Log("[SimpleTeleport] Телепортация завершена.");
    }

    private void OnDrawGizmos()
    {
        if (destination != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destination.position);

            Gizmos.color = Color.red;
            Vector3 launchStart = destination.position;
            Vector3 launchEnd = launchStart + (Vector3)(launchDirection.normalized * 2f);
            Gizmos.DrawLine(launchStart, launchEnd);

            Vector3 arrowTip = launchEnd;
            Vector3 perpendicular = Vector3.Cross(launchDirection.normalized, Vector3.forward).normalized * 0.3f;
            Gizmos.DrawLine(arrowTip, arrowTip - (Vector3)(launchDirection.normalized * 0.5f) + perpendicular);
            Gizmos.DrawLine(arrowTip, arrowTip - (Vector3)(launchDirection.normalized * 0.5f) - perpendicular);
        }
    }
}
