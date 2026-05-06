using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour
{
    [Header("UI Toolkit")]
    public UIController uiController;
    public UIManager uiManager;

    [Header("Settings")]
    public float maxHealth = 100f;

    [Header("Настройки убывания")]
    public float decayRate = 1f;

    private float currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (uiController != null)
        {
            uiController.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    void Update()
    {
        if (isDead) return;

        currentHealth -= decayRate * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (uiController != null)
        {
            uiController.UpdateHealthBar(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (uiController != null)
        {
            uiController.UpdateHealthBar(currentHealth, maxHealth);
        }

        Debug.Log("Полечились на: " + amount + ". Текущее HP: " + currentHealth);
    }

    void GameOver()
    {
        isDead = true;
        Debug.Log("Game Over! Здоровье закончилось.");

        if (uiManager != null)
        {
            uiManager.ShowRestartMenu();
        }

        Time.timeScale = 0f;
    }
}
