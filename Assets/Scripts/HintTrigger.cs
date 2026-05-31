using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HintTrigger : MonoBehaviour
{
    [Header("Настройки подсказки")]
    [TextArea(3, 6)]
    [Tooltip("Текст подсказки, который будет показан игроку")]
    public string hintText = "Используй A, D, Space чтобы двигаться";

    [Tooltip("Длительность показа подсказки в секундах")]
    public float displayDuration = 5f;

    [Tooltip("Показывать подсказку только один раз за уровень")]
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;

    private void Start()
    {
        // Проверяем наличие BoxCollider2D и что он настроен как триггер
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError($"[HintTrigger] На объекте {gameObject.name} отсутствует BoxCollider2D!");
            return;
        }

        if (!boxCollider.isTrigger)
        {
            Debug.LogWarning($"[HintTrigger] BoxCollider2D на объекте {gameObject.name} не настроен как триггер. Устанавливаем Is Trigger = true.");
            boxCollider.isTrigger = true;
        }

        Debug.Log($"[HintTrigger] Триггер подсказки инициализирован на {gameObject.name}. Текст: \"{hintText}\"");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что вошел игрок
        if (collision.CompareTag("Player"))
        {
            // Если подсказка уже была показана и стоит флаг triggerOnlyOnce, выходим
            if (hasTriggered && triggerOnlyOnce)
            {
                Debug.Log($"[HintTrigger] Игрок вошел в триггер {gameObject.name}, но подсказка уже была показана.");
                return;
            }

            // Проверяем наличие DialogueUI синглтона
            if (DialogueUI.Instance == null)
            {
                Debug.LogError("[HintTrigger] DialogueUI.Instance не найден! Убедитесь, что DialogueUI присутствует на сцене.");
                return;
            }

            Debug.Log($"[HintTrigger] Игрок вошел в триггер {gameObject.name}. Показываем подсказку.");

            // Показываем подсказку
            DialogueUI.Instance.ShowHint(hintText, displayDuration);

            // Отмечаем, что триггер сработал
            hasTriggered = true;

            // Если нужно показывать только один раз, отключаем триггер
            if (triggerOnlyOnce)
            {
                BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
                if (boxCollider != null)
                {
                    boxCollider.enabled = false;
                    Debug.Log($"[HintTrigger] Триггер {gameObject.name} отключен после первого срабатывания.");
                }
            }
        }
    }

    private void OnValidate()
    {
        // Проверка в редакторе
        if (displayDuration <= 0)
        {
            Debug.LogWarning($"[HintTrigger] displayDuration должна быть больше 0! Установлено значение по умолчанию 5 сек.");
            displayDuration = 5f;
        }

        if (string.IsNullOrEmpty(hintText))
        {
            Debug.LogWarning($"[HintTrigger] hintText пуст на объекте {gameObject.name}!");
        }
    }
}
