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
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("[DialogueUI] UIDocument компонент не найден!");
            return;
        }

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

        HideDialogue();
        Debug.Log("[DialogueUI] UI элементы найдены и скрыты по умолчанию.");
    }

    public void ShowHint(string text, float duration = 4f)
    {
        if (dialogueBox == null || dialogueText == null)
        {
            Debug.LogError("[DialogueUI] UI элементы не инициализированы!");
            return;
        }

        Debug.Log($"[DialogueUI] Показываем подсказку: \"{text}\" на {duration} сек.");

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        dialogueText.text = text;
        dialogueBox.style.display = DisplayStyle.Flex;
        hideCoroutine = StartCoroutine(HideAfterDelay(duration));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideDialogue();
        Debug.Log("[DialogueUI] Подсказка автоматически скрыта.");
    }

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
