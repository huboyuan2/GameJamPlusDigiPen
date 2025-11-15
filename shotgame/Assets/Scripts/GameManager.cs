using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool isPaused = false;
    public int currentLevel = 0;
    public int score = 0;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "Game";

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        // Hotkey: R to reload level
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadCurrentLevel();
        }

        // Hotkey: ESC to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    #region Scene Management

    // Load main menu scene
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Start the game
    public void StartGame()
    {
        currentLevel = 1;
        score = 0;
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(gameSceneName);
    }

    // Load specified scene
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(sceneName);
    }

    // Reload current level
    public void ReloadCurrentLevel()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Load next level
    public void LoadNextLevel()
    {
        currentLevel++;
        ReloadCurrentLevel();
    }

    #endregion

    #region Game State Management

    // Toggle pause state
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    // Pause the game
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    // Resume the game
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    // Game over
    public void GameOver()
    {
        Debug.Log("Game Over! Score: " + score);
        // Add game over logic here
    }

    // Level completed
    public void LevelComplete()
    {
        Debug.Log("Level Complete!");
        // Add level complete logic here
    }

    #endregion

    #region Score Management

    // Add score
    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Current Score: " + score);
    }

    // Reset score
    public void ResetScore()
    {
        score = 0;
    }

    #endregion

    #region Utility Methods

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion
}