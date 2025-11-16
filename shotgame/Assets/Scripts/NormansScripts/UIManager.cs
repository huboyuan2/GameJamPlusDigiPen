using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject highScore;
    [SerializeField] private GameObject ScoreBoard;
    // Start is called before the first frame update
    void Start()
    {
        GameMenuPhase();
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }
    
    public void GameMenuPhase()
    {
        playButton.SetActive(true);
        highScore.SetActive(false);
        ScoreBoard.SetActive(false);
    }
}
