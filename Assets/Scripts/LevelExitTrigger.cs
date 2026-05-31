using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelExitTrigger : MonoBehaviour
{
    [Header("Настройки победы")]
    [Tooltip("Кастомный текст победы (если пусто, используется стандартный)")]
    public string customWinTitle = "";

    [Tooltip("Показывать ли кнопку следующего уровня (false для финального уровня)")]
    public bool showNextLevelButton = true;

    [Header("Опциональные эффекты")]
    [Tooltip("Звук победы (опционально)")]
    public AudioClip winSound;

    [Tooltip("Частицы победы (опционально)")]
    public ParticleSystem winParticles;

    private bool hasTriggered = false;

    private void Start()
    {
        // Проверяем наличие BoxCollider2D и что он настроен как триггер
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError($"[LevelExitTrigger] На объекте {gameObject.name} отсутствует BoxCollider2D!");
            return;
        }

        if (!boxCollider.isTrigger)
        {
            Debug.LogWarning($"[LevelExitTrigger] BoxCollider2D на объекте {gameObject.name} не настроен как триггер. Устанавливаем Is Trigger = true.");
            boxCollider.isTrigger = true;
        }

        Debug.Log($"[LevelExitTrigger] Триггер выхода из уровня инициализирован на {gameObject.name}.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что вошел игрок и триггер еще не сработал
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log($"[LevelExitTrigger] Игрок достиг выхода из уровня на {gameObject.name}. Показываем меню победы.");

            // Останавливаем абсолютно ВСЕ звуки на игроке и его дочерних объектах
            // Используем GetComponentsInChildren с includeInactive = true
            AudioSource[] allPlayerAudio = collision.GetComponentsInChildren<AudioSource>(true);
            Debug.Log($"[LevelExitTrigger] Найдено AudioSource компонентов: {allPlayerAudio.Length}");

            foreach (AudioSource audio in allPlayerAudio)
            {
                if (audio != null)
                {
                    if (audio.isPlaying)
                    {
                        audio.Stop();
                        Debug.Log($"[LevelExitTrigger] Остановлен AudioSource на: {audio.gameObject.name}");
                    }

                    // КРИТИЧНО: Полностью отключаем AudioSource, чтобы он не мог играть
                    audio.enabled = false;
                    Debug.Log($"[LevelExitTrigger] Отключен AudioSource на: {audio.gameObject.name}");
                }
            }

            // Проигрываем звук победы, если он есть
            if (winSound != null)
            {
                AudioSource.PlayClipAtPoint(winSound, transform.position);
                Debug.Log("[LevelExitTrigger] Проигрываем звук победы.");
            }

            // Запускаем частицы победы, если они есть
            if (winParticles != null)
            {
                winParticles.Play();
                Debug.Log("[LevelExitTrigger] Запускаем частицы победы.");
            }

            // Показываем меню победы
            if (WinMenuUI.Instance != null)
            {
                string titleText = string.IsNullOrEmpty(customWinTitle) ? "УРОВЕНЬ ПРОЙДЕН!" : customWinTitle;
                WinMenuUI.Instance.ShowWinMenu(titleText, showNextLevelButton);
            }
            else
            {
                Debug.LogError("[LevelExitTrigger] WinMenuUI.Instance не найден! Убедитесь, что WinMenuUI присутствует на сцене.");
            }
        }
    }

    private void OnValidate()
    {
        // Проверка в редакторе
        if (!string.IsNullOrEmpty(customWinTitle) && customWinTitle.Length > 50)
        {
            Debug.LogWarning($"[LevelExitTrigger] customWinTitle слишком длинный ({customWinTitle.Length} символов). Рекомендуется до 50 символов.");
        }
    }

    // Визуализация триггера в редакторе
    private void OnDrawGizmos()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.5f); // Зеленый полупрозрачный
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.offset, boxCollider.size);

            Gizmos.color = new Color(0.2f, 1f, 0.3f, 1f); // Зеленый непрозрачный
            Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);
        }
    }
}
