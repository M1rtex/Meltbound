using UnityEngine;

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

    private void ApplyFPSLimit()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && !limitInEditor)
            return;
#endif

        if (SettingsManager.Instance != null)
        {
            Application.targetFrameRate = SettingsManager.Instance.CurrentFPS;
        }
        else
        {
            Application.targetFrameRate = fallbackFPS;
        }

        QualitySettings.vSyncCount = 0;
    }

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
