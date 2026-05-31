using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    private UIDocument uiDocument;
    private VisualElement dialogueBox;
    private Label dialogueText;
    private Coroutine hideCoroutine;

    private void Awake()
    {
        // Синглтон паттерн
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[DialogueUI] Обнаружен дубликат DialogueUI, уничтожаем.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("[DialogueUI] Синглтон инициализирован.");
    }

    private void Start()
    {
        // Получаем UIDocument компонент
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("[DialogueUI] UIDocument компонент не найден!");
            return;
        }

        // Находим элементы UI
        VisualElement root = uiDocument.rootVisualElement;
        dialogueBox = root.Q<VisualElement>("DialogueBox");
        dialogueText = root.Q<Label>("DialogueText");

        if (dialogueBox == null)
        {
            Debug.LogError("[DialogueUI] Элемент #DialogueBox не найден в UXML!");
            return;
        }

        if (dialogueText == null)
        {
            Debug.LogError("[DialogueUI] Элемент #DialogueText не найден в UXML!");
            return;
        }

        // По умолчанию скрываем диалоговое окно
        HideDialogue();
        Debug.Log("[DialogueUI] UI элементы найдены и скрыты по умолчанию.");
    }

    /// <summary>
    /// Показывает подсказку с заданным текстом на указанное время
    /// </summary>
    /// <param name="text">Текст подсказки</param>
    /// <param name="duration">Длительность показа в секундах (по умолчанию 4 секунды)</param>
    public void ShowHint(string text, float duration = 4f)
    {
        if (dialogueBox == null || dialogueText == null)
        {
            Debug.LogError("[DialogueUI] UI элементы не инициализированы!");
            return;
        }

        Debug.Log($"[DialogueUI] Показываем подсказку: \"{text}\" на {duration} сек.");

        // Останавливаем предыдущую корутину скрытия, если она была запущена
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // Устанавливаем текст
        dialogueText.text = text;

        // Показываем диалоговое окно
        dialogueBox.style.display = DisplayStyle.Flex;

        // Запускаем таймер автоматического скрытия
        hideCoroutine = StartCoroutine(HideAfterDelay(duration));
    }

    /// <summary>
    /// Корутина для автоматического скрытия диалога через заданное время
    /// </summary>
    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideDialogue();
        Debug.Log("[DialogueUI] Подсказка автоматически скрыта.");
    }

    /// <summary>
    /// Скрывает диалоговое окно
    /// </summary>
    private void HideDialogue()
    {
        if (dialogueBox != null)
        {
            dialogueBox.style.display = DisplayStyle.None;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Debug.Log("[DialogueUI] Синглтон очищен.");
        }
    }
}
