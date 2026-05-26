using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Documents")]
    public UIDocument mainMenuDocument;
    public UIDocument selectLevelDocument;

    private VisualElement mainMenuPanel;
    private VisualElement selectLevelPanel;

    private Button startButton;
    private Button settingsButton;
    private Button quitButton;
    private Button backButton;

    private Image level1Button;
    private Image level2Button;
    private Image level3Button;
    private Image level4Button;
    private Image level5Button;

    void Awake()
    {
        InitializeMainMenu();
        InitializeSelectLevelMenu();
        HideSelectLevelMenu();
        ShowMainMenu();
    }

    void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void InitializeMainMenu()
    {
        if (mainMenuDocument == null) return;

        var root = mainMenuDocument.rootVisualElement;
        mainMenuPanel = root.Q<VisualElement>("Panel");
        startButton = root.Q<Button>("StartButton");
        settingsButton = root.Q<Button>("SettingsButton");
        quitButton = root.Q<Button>("QuitButton");

        if (startButton != null)
            startButton.clicked += OnStartButtonClicked;

        if (settingsButton != null)
            settingsButton.clicked += OnSettingsButtonClicked;

        if (quitButton != null)
            quitButton.clicked += OnQuitButtonClicked;

        if (mainMenuPanel != null)
            mainMenuPanel.style.display = DisplayStyle.None;
    }

    private void InitializeSelectLevelMenu()
    {
        if (selectLevelDocument == null) return;

        var root = selectLevelDocument.rootVisualElement;
        selectLevelPanel = root.Q<VisualElement>("Panel");
        backButton = root.Q<Button>("Button");

        level1Button = root.Q<Image>("Level1");
        level2Button = root.Q<Image>("Level2");
        level3Button = root.Q<Image>("Level3");
        level4Button = root.Q<Image>("Level4");
        level5Button = root.Q<Image>("Level5");

        if (backButton != null)
            backButton.clicked += OnBackButtonClicked;

        if (level1Button != null)
            level1Button.RegisterCallback<ClickEvent>(evt => OnLevelButtonClicked(1));

        if (level2Button != null)
            level2Button.RegisterCallback<ClickEvent>(evt => OnLevelButtonClicked(2));

        if (level3Button != null)
            level3Button.RegisterCallback<ClickEvent>(evt => OnLevelButtonClicked(3));

        if (level4Button != null)
            level4Button.RegisterCallback<ClickEvent>(evt => OnLevelButtonClicked(4));

        if (level5Button != null)
            level5Button.RegisterCallback<ClickEvent>(evt => OnLevelButtonClicked(5));

        if (selectLevelPanel != null)
            selectLevelPanel.style.display = DisplayStyle.None;
    }

    private void UnsubscribeEvents()
    {
        if (startButton != null)
            startButton.clicked -= OnStartButtonClicked;

        if (settingsButton != null)
            settingsButton.clicked -= OnSettingsButtonClicked;

        if (quitButton != null)
            quitButton.clicked -= OnQuitButtonClicked;

        if (backButton != null)
            backButton.clicked -= OnBackButtonClicked;

        if (level1Button != null)
            level1Button.UnregisterCallback<ClickEvent>(evt => OnLevelButtonClicked(1));

        if (level2Button != null)
            level2Button.UnregisterCallback<ClickEvent>(evt => OnLevelButtonClicked(2));

        if (level3Button != null)
            level3Button.UnregisterCallback<ClickEvent>(evt => OnLevelButtonClicked(3));

        if (level4Button != null)
            level4Button.UnregisterCallback<ClickEvent>(evt => OnLevelButtonClicked(4));

        if (level5Button != null)
            level5Button.UnregisterCallback<ClickEvent>(evt => OnLevelButtonClicked(5));
    }

    private void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked");
        HideMainMenu();
        ShowSelectLevelMenu();
    }

    private void OnSettingsButtonClicked()
    {
        Debug.Log("Настройки пока не реализованы");
    }

    private void OnQuitButtonClicked()
    {
        Debug.Log("Quit button clicked");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void OnBackButtonClicked()
    {
        HideSelectLevelMenu();
        ShowMainMenu();
    }

    private void OnLevelButtonClicked(int levelIndex)
    {
        string sceneName = GetSceneNameForLevel(levelIndex);

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"Сцена для уровня {levelIndex} не найдена!");
        }
    }

    private void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.style.display = DisplayStyle.Flex;
    }

    private void HideMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.style.display = DisplayStyle.None;
    }

    private void ShowSelectLevelMenu()
    {
        if (selectLevelPanel != null)
            selectLevelPanel.style.display = DisplayStyle.Flex;
    }

    private void HideSelectLevelMenu()
    {
        if (selectLevelPanel != null)
            selectLevelPanel.style.display = DisplayStyle.None;
    }

    private string GetSceneNameForLevel(int levelIndex)
    {
        switch (levelIndex)
        {
            case 1:
                return "SampleScene";
            case 2:
                return "TeleportTestScene";
            case 3:
            case 4:
            case 5:
                Debug.LogWarning($"Уровень {levelIndex} еще не создан");
                return null;
            default:
                return null;
        }
    }
}
