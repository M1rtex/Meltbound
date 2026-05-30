using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Управляет UI меню настроек.
/// Синхронизирует слайдеры с SettingsManager и обновляет отображаемые значения.
/// </summary>
public class SettingsMenuUI : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument settingsDocument;

    private SliderInt fpsSlider;
    private Slider soundSlider;
    private Label fpsDataLabel;
    private Label soundDataLabel;

    void OnEnable()
    {
        InitializeUI();
        // Не вызываем LoadCurrentSettings() здесь!
        // Он будет вызван из MainMenuManager при открытии панели
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    /// <summary>
    /// Инициализирует UI элементы и подписывается на события.
    /// </summary>
    private void InitializeUI()
    {
        if (settingsDocument == null)
        {
            Debug.LogError("SettingsDocument не назначен в SettingsMenuUI!");
            return;
        }

        var root = settingsDocument.rootVisualElement;

        // Получаем ссылки на элементы
        fpsSlider = root.Q<SliderInt>("FPSSlider");
        soundSlider = root.Q<Slider>("SoundSlider");
        fpsDataLabel = root.Q<Label>("FPSData");
        soundDataLabel = root.Q<Label>("SoundData");

        // Подписываемся на изменения слайдеров
        if (fpsSlider != null)
        {
            fpsSlider.RegisterValueChangedCallback(OnFPSSliderChanged);
        }

        if (soundSlider != null)
        {
            soundSlider.RegisterValueChangedCallback(OnSoundSliderChanged);
        }
    }

    /// <summary>
    /// Загружает текущие настройки из SettingsManager и обновляет UI.
    /// Принудительно обновляет слайдеры и лейблы при каждом открытии меню.
    /// Вызывается публично из MainMenuManager при открытии панели настроек.
    /// </summary>
    public void LoadCurrentSettings()
    {
        // Безопасный поиск менеджера на случай, если Instance еще не успел проснуться
        if (SettingsManager.Instance == null)
        {
            // Пробуем найти его на сцене принудительно
            var manager = FindObjectOfType<SettingsManager>();
            if (manager == null)
            {
                Debug.LogWarning("[SettingsUI] SettingsManager вообще не найден на сцене!");
                return;
            }
        }

        // Берем актуальные значения из синглтона
        float vol = SettingsManager.Instance.CurrentVolume;
        int fps = SettingsManager.Instance.CurrentFPS;

        Debug.Log($"[SettingsUI] Загружаем настройки: Громкость = {vol}, FPS = {fps}");

        // Берем корневой элемент заново, чтобы исключить баг с неотслеживаемыми изменениями
        if (settingsDocument == null)
        {
            Debug.LogError("[SettingsUI] settingsDocument == null!");
            return;
        }

        var root = settingsDocument.rootVisualElement;

        // Ищем слайдеры и лейблы
        var soundSlider = root.Q<Slider>("SoundSlider");
        var fpsSlider = root.Q<SliderInt>("FPSSlider");
        var soundLabel = root.Q<Label>("SoundData");
        var fpsLabel = root.Q<Label>("FPSData");

        // Проверяем, что все элементы найдены
        if (soundSlider == null) Debug.LogError("[SettingsUI] SoundSlider не найден!");
        if (fpsSlider == null) Debug.LogError("[SettingsUI] FPSSlider не найден!");
        if (soundLabel == null) Debug.LogError("[SettingsUI] SoundData не найден!");
        if (fpsLabel == null) Debug.LogError("[SettingsUI] FPSData не найден!");

        // Устанавливаем значения слайдеров БЕЗ вызова событий
        if (soundSlider != null)
        {
            soundSlider.SetValueWithoutNotify(vol);
            Debug.Log($"[SettingsUI] SoundSlider установлен в {vol}");
        }

        if (fpsSlider != null)
        {
            fpsSlider.SetValueWithoutNotify(fps);
            Debug.Log($"[SettingsUI] FPSSlider установлен в {fps}");
        }

        // Принудительно обновляем текст
        if (soundLabel != null)
        {
            soundLabel.text = $"{Mathf.RoundToInt(vol * 100f)}%";
        }

        if (fpsLabel != null)
        {
            fpsLabel.text = $"{fps} FPS";
        }

        Debug.Log($"[SettingsUI] Визуальные настройки успешно обновлены: Громкость {vol}, FPS {fps}");
    }

    /// <summary>
    /// Обработчик изменения слайдера FPS.
    /// </summary>
    private void OnFPSSliderChanged(ChangeEvent<int> evt)
    {
        int fps = evt.newValue;
        UpdateFPSLabel(fps);

        // Сохраняем и применяем настройки
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetFPS(fps);
        }
    }

    /// <summary>
    /// Обработчик изменения слайдера громкости.
    /// </summary>
    private void OnSoundSliderChanged(ChangeEvent<float> evt)
    {
        float volume = evt.newValue;
        UpdateSoundLabel(volume);

        // Сохраняем и применяем настройки
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetVolume(volume);
        }
    }

    /// <summary>
    /// Обновляет текст лейбла FPS.
    /// </summary>
    private void UpdateFPSLabel(int fps)
    {
        if (fpsDataLabel != null)
        {
            fpsDataLabel.text = $"{fps} FPS";
        }
    }

    /// <summary>
    /// Обновляет текст лейбла громкости.
    /// </summary>
    private void UpdateSoundLabel(float volume)
    {
        if (soundDataLabel != null)
        {
            int percentage = Mathf.RoundToInt(volume * 100f);
            soundDataLabel.text = $"{percentage}%";
        }
    }

    /// <summary>
    /// Отписывается от событий при отключении.
    /// </summary>
    private void UnsubscribeEvents()
    {
        if (fpsSlider != null)
        {
            fpsSlider.UnregisterValueChangedCallback(OnFPSSliderChanged);
        }

        if (soundSlider != null)
        {
            soundSlider.UnregisterValueChangedCallback(OnSoundSliderChanged);
        }
    }
}
