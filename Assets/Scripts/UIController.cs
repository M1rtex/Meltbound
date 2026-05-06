using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private ProgressBar healthBar;

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
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            // Debug.LogWarning($"Health bar is on {currentHealth}/{maxHealth}");
            healthBar.value = currentHealth;
            healthBar.highValue = maxHealth;
        }
    }
}
