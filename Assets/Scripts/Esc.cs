using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 需要引入 SceneManagement

public class Exit : MonoBehaviour
{

    public GameObject exitUi; // Reference to your Exit UI GameObject
    private bool isPaused = false;

    void Start()
    {
        // Make sure the UI is hidden at start
        if (exitUi != null)
        {
            exitUi.SetActive(false);
        }
    }

    void Update()
    {
        // Check for Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        // Show the exit UI
        if (exitUi != null)
        {
            exitUi.SetActive(true);
        }

        // Pause the game
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        // Hide the exit UI
        if (exitUi != null)
        {
            exitUi.SetActive(false);
        }

        // Resume the game
        Time.timeScale = 1f;
        isPaused = false;
    }

    // 当场景切换时自动恢复时间
    void OnDestroy()
    {
        Time.timeScale = 1f; // 确保切换场景后时间恢复正常
    }
}