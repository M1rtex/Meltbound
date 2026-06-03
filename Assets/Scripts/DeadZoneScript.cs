using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    public UIManager uiManager;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip gameOverSound; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            GameOver();
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        if (uiManager != null)
        {
            uiManager.ShowRestartMenu();
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
}