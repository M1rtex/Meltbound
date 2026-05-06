using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Documents")]
    public UIDocument gameUIDocument;
    public UIDocument pauseMenuDocument;
    public UIDocument startMenuDocument;
    public UIDocument restartMenuDocument;

    private static bool isRestarting = false;

    private VisualElement pauseOverlay;
    private VisualElement startOverlay;
    private VisualElement restartOverlay;

    private Button pauseButton;
    private Button restartButton;
    private Button resumeButton;
    private Button quitButton;
    private Button startButton;
    private Button restartButtonMenu;
    private Button quitButtonMenu;

    void Awake()
    {
        InitializeGameUI();
        InitializePauseMenu();
        InitializeStartMenu();
        InitializeRestartMenu();

        if (!isRestarting)
        {
            ShowStartMenu();
        }
        else
        {
            HideAllMenus();
            Time.timeScale = 1f;
            isRestarting = false;
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

        if (pauseButton != null)
            pauseButton.clicked += OnPausePress;

        if (restartButton != null)
            restartButton.clicked += OnRestartPress;
    }

    private void InitializePauseMenu()
    {
        if (pauseMenuDocument == null) return;

        var root = pauseMenuDocument.rootVisualElement;
        pauseOverlay = root.Q<VisualElement>("pause-overlay");
        resumeButton = root.Q<Button>("resume-button");
        quitButton = root.Q<Button>("quit-button");

        if (resumeButton != null)
            resumeButton.clicked += OnResumePress;

        if (quitButton != null)
            quitButton.clicked += OnRageQuitPress;

        if (pauseOverlay != null)
            pauseOverlay.style.display = DisplayStyle.None;
    }

    private void InitializeStartMenu()
    {
        if (startMenuDocument == null) return;

        var root = startMenuDocument.rootVisualElement;
        startOverlay = root.Q<VisualElement>("start-overlay");
        startButton = root.Q<Button>("start-button");

        if (startButton != null)
            startButton.clicked += OnStartPress;

        if (startOverlay != null)
            startOverlay.style.display = DisplayStyle.None;
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

        if (quitButton != null)
            quitButton.clicked -= OnRageQuitPress;

        if (startButton != null)
            startButton.clicked -= OnStartPress;

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

    public void OnStartPress()
    {
        HideStartMenu();
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

    private void ShowStartMenu()
    {
        if (startOverlay != null)
        {
            startOverlay.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f;
        }
    }

    private void HideStartMenu()
    {
        if (startOverlay != null)
        {
            startOverlay.style.display = DisplayStyle.None;
            Time.timeScale = 1f;
        }
    }

    private void HideAllMenus()
    {
        if (pauseOverlay != null)
            pauseOverlay.style.display = DisplayStyle.None;

        if (startOverlay != null)
            startOverlay.style.display = DisplayStyle.None;

        if (restartOverlay != null)
            restartOverlay.style.display = DisplayStyle.None;
    }
}
