using UnityEngine;

/// <summary>
/// Применяет ограничение FPS из SettingsManager.
/// Если SettingsManager не найден, использует значение по умолчанию.
/// </summary>
public class FPSLimiter : MonoBehaviour
{
    [Header("FPS Settings")]
    [SerializeField] private int fallbackFPS = 60;
    [SerializeField] private bool limitInEditor = true;

    private void Awake()
    {
        ApplyFPSLimit();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            ApplyFPSLimit();
        }
#if UNITY_EDITOR
        else if (limitInEditor)
        {
            ApplyFPSLimitFallback();
        }
#endif
    }

    /// <summary>
    /// Применяет ограничение FPS из SettingsManager.
    /// </summary>
    private void ApplyFPSLimit()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && !limitInEditor)
            return;
#endif

        // Пытаемся получить FPS из SettingsManager
        if (SettingsManager.Instance != null)
        {
            Application.targetFrameRate = SettingsManager.Instance.CurrentFPS;
        }
        else
        {
            // Если SettingsManager еще не инициализирован, используем fallback
            Application.targetFrameRate = fallbackFPS;
        }

        QualitySettings.vSyncCount = 0;
    }

    /// <summary>
    /// Применяет fallback значение FPS (для редактора).
    /// </summary>
    private void ApplyFPSLimitFallback()
    {
        Application.targetFrameRate = fallbackFPS;
        QualitySettings.vSyncCount = 0;
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    private static void InitializeInEditor()
    {
        UnityEditor.EditorApplication.update += UpdateInEditor;
    }

    private static void UpdateInEditor()
    {
        FPSLimiter limiter = FindFirstObjectByType<FPSLimiter>();
        if (limiter != null && limiter.limitInEditor && !Application.isPlaying)
        {
            Application.targetFrameRate = limiter.fallbackFPS;
            QualitySettings.vSyncCount = 0;
        }
    }
#endif
}
