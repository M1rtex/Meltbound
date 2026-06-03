using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Audio Settings")]
    [Tooltip("Прикрепите сюда AudioMixer из папки Assets/Audio")]
    [SerializeField] private AudioMixer audioMixer;

    private const string VOLUME_KEY = "Volume";
    private const string FPS_KEY = "FPS";

    private const float DEFAULT_VOLUME = 1.0f;
    private const int DEFAULT_FPS = 60;

    private float currentVolume;
    private int currentFPS;

    public float CurrentVolume => currentVolume;
    public int CurrentFPS => currentFPS;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            LoadSettings();
        }
    }

    private void Start()
    {
        StartCoroutine(ApplySettingsNextFrame());
    }

    private System.Collections.IEnumerator ApplySettingsNextFrame()
    {
        yield return null;
        ApplySettings();
    }

    private void LoadSettings()
    {
        currentVolume = PlayerPrefs.GetFloat(VOLUME_KEY, DEFAULT_VOLUME);
        currentFPS = PlayerPrefs.GetInt(FPS_KEY, DEFAULT_FPS);
    }

    public void SaveSettings(float volume, int fps)
    {
        currentVolume = Mathf.Clamp01(volume);
        currentFPS = Mathf.Clamp(fps, 24, 240);

        PlayerPrefs.SetFloat(VOLUME_KEY, currentVolume);
        PlayerPrefs.SetInt(FPS_KEY, currentFPS);
        PlayerPrefs.Save();

        ApplySettings();
    }

    public void ApplySettings()
    {
        ApplyFPSSettings();
        ApplyVolumeSettings();
    }

    private void ApplyFPSSettings()
    {
        Application.targetFrameRate = currentFPS;
        QualitySettings.vSyncCount = 1;
    }

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
            db = -80f;
        }
        else
        {
            db = Mathf.Log10(currentVolume) * 20f;
        }

        audioMixer.SetFloat("MasterVolume", db);
    }

    public void SetVolume(float volume)
    {
        SaveSettings(volume, currentFPS);
    }

    public void SetFPS(int fps)
    {
        SaveSettings(currentVolume, fps);
    }
}
