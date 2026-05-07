using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private ProgressBar healthBar;
    private VisualElement healthBarProgress;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component not found!");
            return;
        }

        var root = uiDocument.rootVisualElement;
        healthBar = root.Q<ProgressBar>("health-bar");

        if (healthBar == null)
        {
            Debug.LogError("Health bar not found in UI!");
            return;
        }

        healthBarProgress = healthBar.Q<VisualElement>(className: "unity-progress-bar__progress");

        if (healthBarProgress == null)
        {
            Debug.LogError("Health bar progress element not found!");
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
            healthBar.highValue = maxHealth;

            if (healthBarProgress != null)
            {
                float healthPercent = currentHealth / maxHealth;
                Color healthColor = GetHealthColor(healthPercent);
                healthBarProgress.style.backgroundColor = new StyleColor(healthColor);
            }
        }
    }

    private Color GetHealthColor(float healthPercent)
    {
        if (healthPercent > 0.75f)
        {
            return Color.Lerp(new Color(0.39f, 0.78f, 1f), new Color(0.2f, 0.59f, 1f), (healthPercent - 0.75f) / 0.25f);
        }
        else if (healthPercent > 0.5f)
        {
            return Color.Lerp(new Color(1f, 0.86f, 0.39f), new Color(0.39f, 0.78f, 1f), (healthPercent - 0.5f) / 0.25f);
        }
        else if (healthPercent > 0.25f)
        {
            return Color.Lerp(new Color(1f, 0.55f, 0.24f), new Color(1f, 0.86f, 0.39f), (healthPercent - 0.25f) / 0.25f);
        }
        else
        {
            return Color.Lerp(new Color(0.86f, 0.2f, 0.2f), new Color(1f, 0.55f, 0.24f), healthPercent / 0.25f);
        }
    }
}
