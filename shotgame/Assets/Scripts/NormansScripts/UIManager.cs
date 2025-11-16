using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject highScore;
    [SerializeField] private GameObject ScoreBoard;


    public static Action GameStart;
    void OnEnable()
    {
        // Subscribe to player death event
        Health.PlayerDead += OnPlayerDeath;
    }

    void OnDisable()
    {
        // Unsubscribe from player death event
        Health.PlayerDead -= OnPlayerDeath;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameMenuPhase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called when player dies
    private void OnPlayerDeath()
    {
        Debug.Log("[UIManager] Player death detected, calling GameEndPhase()");
        GameEndPhase();
    }
    
    public void GameEndPhase()
    {
        playButton.SetActive(false);
        highScore.SetActive(false);
        ScoreBoard.SetActive(true);
    }
    
    public void GameStartPhase()
    {
        playButton.SetActive(false);
        highScore.SetActive(true);
        ScoreBoard.SetActive(false);
        GameStart?.Invoke();
    }
    
    public void GameMenuPhase()
    {
        playButton.SetActive(true);
        highScore.SetActive(false);
        ScoreBoard.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
