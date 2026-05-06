using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    public UIManager uiManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            GameOver();
    }

    void GameOver()
    {
        Debug.Log("Game Over!");

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