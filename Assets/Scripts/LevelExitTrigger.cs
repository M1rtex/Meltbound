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
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log($"[LevelExitTrigger] Игрок достиг выхода из уровня на {gameObject.name}. Показываем меню победы.");

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

                    audio.enabled = false;
                    Debug.Log($"[LevelExitTrigger] Отключен AudioSource на: {audio.gameObject.name}");
                }
            }

            if (winSound != null)
            {
                AudioSource.PlayClipAtPoint(winSound, transform.position);
                Debug.Log("[LevelExitTrigger] Проигрываем звук победы.");
            }

            if (winParticles != null)
            {
                winParticles.Play();
                Debug.Log("[LevelExitTrigger] Запускаем частицы победы.");
            }

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
        if (!string.IsNullOrEmpty(customWinTitle) && customWinTitle.Length > 50)
        {
            Debug.LogWarning($"[LevelExitTrigger] customWinTitle слишком длинный ({customWinTitle.Length} символов). Рекомендуется до 50 символов.");
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.5f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.offset, boxCollider.size);

            Gizmos.color = new Color(0.2f, 1f, 0.3f, 1f);
            Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);
        }
    }
}
