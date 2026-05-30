using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI Documents")]
    public UIDocument gameUIDocument;
    public UIDocument pauseMenuDocument;
    public UIDocument restartMenuDocument;

    private static bool isRestarting = false;

    private VisualElement pauseOverlay;
    private VisualElement restartOverlay;

    private Label levelBanner;

    private Button pauseButton;
    private Button restartButton;
    private Button resumeButton;
    private Button toMenuButton;
    private Button quitButton;
    private Button restartButtonMenu;
    private Button quitButtonMenu;

    void Awake()
    {
        InitializeGameUI();
        InitializePauseMenu();
        InitializeRestartMenu();

        // Игра начинается сразу, без заставки
        Time.timeScale = 1f;

        if (isRestarting)
        {
            isRestarting = false;
        }
    }

    void Start()
    {
        // Запускаем анимацию баннера уровня
        if (levelBanner != null)
        {
            StartCoroutine(ShowLevelBanner());
        }
    }

    void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void InitializeGameUI()
    {
        if (gameUIDocument == null) return;

        var root = gameUIDocument.rootVisualElement;
        pauseButton = root.Q<Button>("pause-button");
        restartButton = root.Q<Button>("restart-button");
        levelBanner = root.Q<Label>("level-banner");

        if (pauseButton != null)
            pauseButton.clicked += OnPausePress;

        if (restartButton != null)
            restartButton.clicked += OnRestartPress;

        // Определяем номер уровня из индекса сцены
        if (levelBanner != null)
        {
            int levelIndex = SceneManager.GetActiveScene().buildIndex;
            // Если первая сцена (индекс 0) — это главное меню, вычитаем 1
            int levelNumber = levelIndex > 0 ? levelIndex : 1;
            levelBanner.text = $"Уровень {levelNumber}";
        }
    }

    private void InitializePauseMenu()
    {
        if (pauseMenuDocument == null) return;

        var root = pauseMenuDocument.rootVisualElement;
        pauseOverlay = root.Q<VisualElement>("pause-overlay");
        resumeButton = root.Q<Button>("resume-button");
        toMenuButton = root.Q<Button>("to-menu-button");
        quitButton = root.Q<Button>("quit-button");

        if (resumeButton != null)
            resumeButton.clicked += OnResumePress;

        if (toMenuButton != null)
            toMenuButton.clicked += LoadMainMenu;

        if (quitButton != null)
            quitButton.clicked += OnRageQuitPress;

        if (pauseOverlay != null)
            pauseOverlay.style.display = DisplayStyle.None;
    }

    private void InitializeRestartMenu()
    {
        if (restartMenuDocument == null) return;

        var root = restartMenuDocument.rootVisualElement;
        restartOverlay = root.Q<VisualElement>("restart-overlay");
        restartButtonMenu = root.Q<Button>("restart-button-menu");
        quitButtonMenu = root.Q<Button>("quit-button-menu");

        if (restartButtonMenu != null)
            restartButtonMenu.clicked += OnRestartPress;

        if (quitButtonMenu != null)
            quitButtonMenu.clicked += OnRageQuitPress;

        if (restartOverlay != null)
            restartOverlay.style.display = DisplayStyle.None;
    }

    private void UnsubscribeEvents()
    {
        if (pauseButton != null)
            pauseButton.clicked -= OnPausePress;

        if (restartButton != null)
            restartButton.clicked -= OnRestartPress;

        if (resumeButton != null)
            resumeButton.clicked -= OnResumePress;

        if (toMenuButton != null)
            toMenuButton.clicked -= LoadMainMenu;

        if (quitButton != null)
            quitButton.clicked -= OnRageQuitPress;

        if (restartButtonMenu != null)
            restartButtonMenu.clicked -= OnRestartPress;

        if (quitButtonMenu != null)
            quitButtonMenu.clicked -= OnRageQuitPress;
    }

    public void OnRestartPress()
    {
        isRestarting = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnRageQuitPress()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnPausePress()
    {
        ShowPauseMenu();
    }

    public void OnResumePress()
    {
        HidePauseMenu();
    }

    public void ShowRestartMenu()
    {
        if (restartOverlay != null)
        {
            restartOverlay.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f;
        }
    }

    private void ShowPauseMenu()
    {
        if (pauseOverlay != null)
        {
            pauseOverlay.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f;
        }
    }

    private void HidePauseMenu()
    {
        if (pauseOverlay != null)
        {
            pauseOverlay.style.display = DisplayStyle.None;
            Time.timeScale = 1f;
        }
    }

    private void HideAllMenus()
    {
        if (pauseOverlay != null)
            pauseOverlay.style.display = DisplayStyle.None;

        if (restartOverlay != null)
            restartOverlay.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Загружает сцену главного меню.
    /// Перед загрузкой восстанавливает нормальный ход времени.
    /// </summary>
    private void LoadMainMenu()
    {
        // Восстанавливаем нормальный ход времени перед загрузкой сцены
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary>
    /// Показывает баннер уровня и плавно скрывает его через 2 секунды.
    /// </summary>
    private IEnumerator ShowLevelBanner()
    {
        if (levelBanner == null) yield break;

        // Показываем баннер на 2 секунды
        yield return new WaitForSeconds(2f);

        // Плавно уменьшаем прозрачность
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float opacity = Mathf.Lerp(1f, 0f, elapsed / duration);
            levelBanner.style.opacity = opacity;
            yield return null;
        }

        // Полностью скрываем элемент
        levelBanner.style.opacity = 0f;
        levelBanner.style.display = DisplayStyle.None;
    }
}
