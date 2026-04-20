using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    public GameObject RestartUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            RestartUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}