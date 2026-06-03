using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument settingsDocument;

    private SliderInt fpsSlider;
    private Slider soundSlider;
    private Label fpsDataLabel;
    private Label soundDataLabel;

    private void OnEnable()
    {
        LoadCurrentSettings();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void LoadCurrentSettings()
    {
        UnsubscribeEvents();

        fpsSlider = null;
        soundSlider = null;
        fpsDataLabel = null;
        soundDataLabel = null;

        if (settingsDocument == null)
        {
            settingsDocument = GetComponent<UIDocument>();
        }

        if (settingsDocument == null)
        {
            Debug.LogError("[SettingsMenuUI] UIDocument не найден на объекте!");
            return;
        }

        var root = settingsDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("[SettingsMenuUI] rootVisualElement == null!");
            return;
        }

        fpsSlider = root.Q<SliderInt>("FPSSlider");
        soundSlider = root.Q<Slider>("SoundSlider");
        fpsDataLabel = root.Q<Label>("FPSData");
        soundDataLabel = root.Q<Label>("SoundData");

        if (fpsSlider == null) Debug.LogError("[SettingsMenuUI] FPSSlider не найден!");
        if (soundSlider == null) Debug.LogError("[SettingsMenuUI] SoundSlider не найден!");
        if (fpsDataLabel == null) Debug.LogError("[SettingsMenuUI] FPSData не найден!");
        if (soundDataLabel == null) Debug.LogError("[SettingsMenuUI] SoundData не найден!");

        if (SettingsManager.Instance == null)
        {
            Debug.LogWarning("[SettingsMenuUI] SettingsManager.Instance == null! Это не должно происходить. Убедитесь, что SettingsManager существует на сцене.");
            return;
        }

        float vol = SettingsManager.Instance.CurrentVolume;
        int fps = SettingsManager.Instance.CurrentFPS;

        if (soundSlider != null)
        {
            soundSlider.SetValueWithoutNotify(vol);
        }

        if (fpsSlider != null)
        {
            fpsSlider.SetValueWithoutNotify(fps);
        }

        UpdateSoundLabel(vol);
        UpdateFPSLabel(fps);

        if (fpsSlider != null)
        {
            fpsSlider.RegisterValueChangedCallback(OnFPSSliderChanged);
        }

        if (soundSlider != null)
        {
            soundSlider.RegisterValueChangedCallback(OnSoundSliderChanged);
        }
    }

    private void OnFPSSliderChanged(ChangeEvent<int> evt)
    {
        int fps = evt.newValue;
        UpdateFPSLabel(fps);

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetFPS(fps);
        }
    }

    private void OnSoundSliderChanged(ChangeEvent<float> evt)
    {
        float volume = evt.newValue;
        UpdateSoundLabel(volume);

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetVolume(volume);
        }
    }

    private void UpdateFPSLabel(int fps)
    {
        if (fpsDataLabel != null)
        {
            fpsDataLabel.text = $"{fps} FPS";
        }
    }

    private void UpdateSoundLabel(float volume)
    {
        if (soundDataLabel != null)
        {
            int percentage = Mathf.RoundToInt(volume * 100f);
            soundDataLabel.text = $"{percentage}%";
        }
    }

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
