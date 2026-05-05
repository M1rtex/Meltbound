using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public GameObject RestartUI;
    public float maxHealth = 100f;
    
    [Header("Настройки убывания")]
    public float decayRate = 1f; 
    
    private float currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    void Update()
    {
        if (isDead) return; 
        currentHealth -= decayRate * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        slider.value = currentHealth;
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
        slider.value = currentHealth;
        Debug.Log("Полечились на: " + amount + ". Текущее HP: " + currentHealth);
    }

    void GameOver()
    {
        isDead = true;
        Debug.Log("Game Over! Здоровье закончилось.");
        RestartUI.SetActive(true);
        Time.timeScale = 0f;
    }
}