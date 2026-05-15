using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class HealVignetteEffect : MonoBehaviour
{
    [Header("Настройки виньетки")]
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private VisualElement vignetteElement;
    private Coroutine currentEffect;

    private void Awake()
    {
        if (uiDocument == null)
        {
            uiDocument = FindObjectOfType<UIDocument>();
        }

        if (uiDocument != null)
        {
            var root = uiDocument.rootVisualElement;
            vignetteElement = root.Q<VisualElement>("heal-vignette");

            if (vignetteElement == null)
            {
                Debug.LogWarning("Heal vignette element not found in UI! Make sure 'heal-vignette' exists in GameUI.uxml");
            }
            else
            {
                vignetteElement.style.opacity = 0f;
            }
        }
    }

    public void TriggerHealEffect()
    {
        if (currentEffect != null)
        {
            StopCoroutine(currentEffect);
        }
        currentEffect = StartCoroutine(HealEffectCoroutine());
    }

    private IEnumerator HealEffectCoroutine()
    {
        if (vignetteElement == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;
            vignetteElement.style.opacity = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        vignetteElement.style.opacity = 1f;

        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            vignetteElement.style.opacity = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        vignetteElement.style.opacity = 0f;
        currentEffect = null;
    }
}
