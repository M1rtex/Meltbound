using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class WinMenuUI : MonoBehaviour
{
    public static WinMenuUI Instance { get; private set; }

    private UIDocument uiDocument;
    private VisualElement winPanel;
    private Label winTitle;
    private Button nextLevelButton;
    private Button mainMenuButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[WinMenuUI] Обнаружен дубликат WinMenuUI, уничтожаем.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("[WinMenuUI] Синглтон инициализирован.");
    }

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("[WinMenuUI] UIDocument компонент не найден!");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;
        winPanel = root.Q<VisualElement>("WinPanel");
        winTitle = root.Q<Label>("WinTitle");
        nextLevelButton = root.Q<Button>("NextLevelButton");
        mainMenuButton = root.Q<Button>("MainMenuButton");

        if (winPanel == null)
        {
            Debug.LogError("[WinMenuUI] Элемент #WinPanel не найден в UXML!");
            return;
        }

        if (winTitle == null)
        {
            Debug.LogError("[WinMenuUI] Элемент #WinTitle не найден в UXML!");
        }

        if (nextLevelButton == null)
        {
            Debug.LogError("[WinMenuUI] Элемент #NextLevelButton не найден в UXML!");
        }

        if (mainMenuButton == null)
        {
            Debug.LogError("[WinMenuUI] Элемент #MainMenuButton не найден в UXML!");
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.RegisterCallback<ClickEvent>(OnNextLevelClicked);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.RegisterCallback<ClickEvent>(OnMainMenuClicked);
        }

        HideWinMenu();
        Debug.Log("[WinMenuUI] UI элементы найдены и меню скрыто по умолчанию.");
    }

    public void ShowWinMenu(string customTitle = "УРОВЕНЬ ПРОЙДЕН!", bool showNextButton = true)
    {
        if (winPanel == null)
        {
            Debug.LogError("[WinMenuUI] UI элементы не инициализированы!");
            return;
        }

        Debug.Log($"[WinMenuUI] Показываем меню победы. Заголовок: \"{customTitle}\", Кнопка следующего уровня: {showNextButton}");

        if (winTitle != null)
        {
            winTitle.text = customTitle;
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.style.display = showNextButton ? DisplayStyle.Flex : DisplayStyle.None;
        }

        winPanel.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;

        Debug.Log("[WinMenuUI] Меню победы показано. Время остановлено.");
    }

    public void HideWinMenu()
    {
        if (winPanel != null)
        {
            winPanel.style.display = DisplayStyle.None;
            Debug.Log("[WinMenuUI] Меню победы скрыто.");
        }

        Time.timeScale = 1f;
    }

    private void OnNextLevelClicked(ClickEvent evt)
    {
        Debug.Log("[WinMenuUI] Нажата кнопка 'Следующий уровень'.");

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        Debug.Log($"[WinMenuUI] Текущая сцена: {currentSceneIndex}, Следующая: {nextSceneIndex}, Всего сцен в Build Settings: {totalScenes}");

        if (nextSceneIndex < totalScenes)
        {
            Time.timeScale = 1f;
            Debug.Log($"[WinMenuUI] Загружаем следующую сцену с индексом {nextSceneIndex}.");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning($"[WinMenuUI] Следующая сцена с индексом {nextSceneIndex} не существует! Возвращаемся в главное меню.");
            Time.timeScale = 1f;
            LoadMainMenu();
        }
    }

    private void OnMainMenuClicked(ClickEvent evt)
    {
        Debug.Log("[WinMenuUI] Нажата кнопка 'Главное меню'.");
        Time.timeScale = 1f;
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        Debug.Log("[WinMenuUI] Загружаем главное меню (сцена с индексом 0).");
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.UnregisterCallback<ClickEvent>(OnNextLevelClicked);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.UnregisterCallback<ClickEvent>(OnMainMenuClicked);
        }

        if (Instance == this)
        {
            Instance = null;
            Debug.Log("[WinMenuUI] Синглтон очищен.");
        }

        Time.timeScale = 1f;
    }
}
