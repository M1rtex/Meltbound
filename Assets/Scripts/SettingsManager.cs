using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Синглтон для управления настройками игры (FPS, громкость).
/// Сохраняет настройки в PlayerPrefs и применяет их ко всей игре.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Audio Settings")]
    [Tooltip("Прикрепите сюда AudioMixer из папки Assets/Audio")]
    [SerializeField] private AudioMixer audioMixer;

    // Ключи для сохранения в PlayerPrefs
    private const string VOLUME_KEY = "Volume";
    private const string FPS_KEY = "FPS";

    // Значения по умолчанию
    private const float DEFAULT_VOLUME = 1.0f;
    private const int DEFAULT_FPS = 60;

    // Текущие значения настроек
    private float currentVolume;
    private int currentFPS;

    public float CurrentVolume => currentVolume;
    public int CurrentFPS => currentFPS;

    void Awake()
    {
        // Реализация синглтона
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Загружаем сохраненные настройки
        LoadSettings();
    }

    void Start()
    {
        // Запускаем безопасное применение настроек со следующего кадра
        // AudioMixer часто игнорирует изменения в самый первый кадр
        StartCoroutine(ApplySettingsNextFrame());
    }

    /// <summary>
    /// Применяет настройки со следующего кадра после полной инициализации Unity.
    /// </summary>
    private System.Collections.IEnumerator ApplySettingsNextFrame()
    {
        // Пропускаем самый первый кадр, пока всё инициализируется
        yield return null;

        // Теперь жестко применяем и FPS, и громкость микшера
        ApplySettings();

        Debug.Log($"Настройки применены при старте: Громкость = {currentVolume}, FPS = {currentFPS}");
    }

    /// <summary>
    /// Загружает настройки из PlayerPrefs.
    /// Если настроек нет (первый запуск), использует значения по умолчанию.
    /// </summary>
    private void LoadSettings()
    {
        currentVolume = PlayerPrefs.GetFloat(VOLUME_KEY, DEFAULT_VOLUME);
        currentFPS = PlayerPrefs.GetInt(FPS_KEY, DEFAULT_FPS);

        Debug.Log($"Настройки загружены: Громкость = {currentVolume}, FPS = {currentFPS}");
    }

    /// <summary>
    /// Сохраняет настройки в PlayerPrefs и применяет их.
    /// </summary>
    public void SaveSettings(float volume, int fps)
    {
        currentVolume = Mathf.Clamp01(volume);
        currentFPS = Mathf.Clamp(fps, 24, 240);

        PlayerPrefs.SetFloat(VOLUME_KEY, currentVolume);
        PlayerPrefs.SetInt(FPS_KEY, currentFPS);

        // Принудительно сохраняем на диск
        PlayerPrefs.Save();

        Debug.Log($"Настройки сохранены: Громкость = {currentVolume}, FPS = {currentFPS}");

        ApplySettings();
    }

    /// <summary>
    /// Применяет текущие настройки к игре.
    /// </summary>
    public void ApplySettings()
    {
        ApplyFPSSettings();
        ApplyVolumeSettings();
    }

    /// <summary>
    /// Применяет настройку FPS.
    /// </summary>
    private void ApplyFPSSettings()
    {
        Application.targetFrameRate = currentFPS;
        QualitySettings.vSyncCount = 0;
    }

    /// <summary>
    /// Применяет настройку громкости к AudioMixer.
    /// Преобразует линейное значение (0-1) в децибелы (-80 до 0).
    /// </summary>
    private void ApplyVolumeSettings()
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("AudioMixer не назначен в SettingsManager! Прикрепите его в инспекторе.");
            return;
        }

        float db;
        if (currentVolume <= 0f)
        {
            db = -80f; // Полная тишина
        }
        else
        {
            // Преобразование линейного значения в децибелы
            db = Mathf.Log10(currentVolume) * 20f;
        }

        audioMixer.SetFloat("MasterVolume", db);
    }

    /// <summary>
    /// Устанавливает только громкость и сохраняет настройки.
    /// </summary>
    public void SetVolume(float volume)
    {
        SaveSettings(volume, currentFPS);
    }

    /// <summary>
    /// Устанавливает только FPS и сохраняет настройки.
    /// </summary>
    public void SetFPS(int fps)
    {
        SaveSettings(currentVolume, fps);
    }
}
