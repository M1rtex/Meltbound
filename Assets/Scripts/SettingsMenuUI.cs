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
        // При включении панели сразу подтягиваем свежие данные и ищем элементы текущей сцены
        LoadCurrentSettings();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    /// <summary>
    /// Загружает текущие настройки из SettingsManager и обновляет UI.
    /// Принудительно обновляет слайдеры и лейблы при каждом открытии меню.
    /// Вызывается публично из MainMenuManager при открытии панели настроек.
    /// </summary>
    public void LoadCurrentSettings()
    {
        // 1. ЗАБЫВАЕМ ПРО СТАРОЕ СОСТОЯНИЕ - отписываемся от старых событий
        UnsubscribeEvents();

        // 2. ОБНУЛЯЕМ все ссылки на старые UI элементы
        fpsSlider = null;
        soundSlider = null;
        fpsDataLabel = null;
        soundDataLabel = null;

        // 3. ПРИНУДИТЕЛЬНО получаем свежий UIDocument
        if (settingsDocument == null)
        {
            settingsDocument = GetComponent<UIDocument>();
        }

        if (settingsDocument == null)
        {
            Debug.LogError("[SettingsMenuUI] UIDocument не найден на объекте!");
            return;
        }

        // 4. ПОЛУЧАЕМ СВЕЖИЙ ROOT из актуальной сцены
        var root = settingsDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("[SettingsMenuUI] rootVisualElement == null!");
            return;
        }

        // 5. ПЕРЕНАХОДИМ элементы заново на сцене
        fpsSlider = root.Q<SliderInt>("FPSSlider");
        soundSlider = root.Q<Slider>("SoundSlider");
        fpsDataLabel = root.Q<Label>("FPSData");
        soundDataLabel = root.Q<Label>("SoundData");

        // Проверяем, что все элементы найдены
        if (fpsSlider == null) Debug.LogError("[SettingsMenuUI] FPSSlider не найден!");
        if (soundSlider == null) Debug.LogError("[SettingsMenuUI] SoundSlider не найден!");
        if (fpsDataLabel == null) Debug.LogError("[SettingsMenuUI] FPSData не найден!");
        if (soundDataLabel == null) Debug.LogError("[SettingsMenuUI] SoundData не найден!");

        // 6. БЕЗОПАСНЫЙ ПОИСК МЕНЕДЖЕРА
        if (SettingsManager.Instance == null)
        {
            Debug.LogWarning("[SettingsMenuUI] SettingsManager.Instance == null! Это не должно происходить. Убедитесь, что SettingsManager существует на сцене.");
            return;
        }

        // 7. БЕРЕМ АКТУАЛЬНЫЕ ЗНАЧЕНИЯ из менеджера
        float vol = SettingsManager.Instance.CurrentVolume;
        int fps = SettingsManager.Instance.CurrentFPS;

        // 8. УСТАНАВЛИВАЕМ ЗНАЧЕНИЯ БЕЗ УВЕДОМЛЕНИЯ (SetValueWithoutNotify)
        if (soundSlider != null)
        {
            soundSlider.SetValueWithoutNotify(vol);
        }

        if (fpsSlider != null)
        {
            fpsSlider.SetValueWithoutNotify(fps);
        }

        // 9. ОБНОВЛЯЕМ ЛЕЙБЛЫ
        UpdateSoundLabel(vol);
        UpdateFPSLabel(fps);

        // 10. СТРОГО ПОСЛЕ ЭТОГО ПОДПИСЫВАЕМСЯ НА СОБЫТИЯ
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
    /// Отписывается от событий слайдеров.
    /// Безопасно работает даже если элементы == null.
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
