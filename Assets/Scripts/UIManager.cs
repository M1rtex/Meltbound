using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class UIManager : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject startUI;

    private static bool isRestarting = false;

    public void Awake()
    {
        if (!isRestarting)
        {
            startUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            startUI.SetActive(false);
            Time.timeScale = 1f;
            
            isRestarting = false;
        }
    }

    public void OnRestartPress()
    {
        isRestarting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void OnRageQuitPress()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnPausePress()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnResumePress()
    {
        pauseUI.SetActive(false);
        startUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnStartPress()
    {
        startUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
