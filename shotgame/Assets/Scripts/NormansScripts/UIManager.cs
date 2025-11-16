using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject highScore;
    // Start is called before the first frame update
    void Start()
    {
        GameMenuPhase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStartPhase()
    {
        playButton.SetActive(false);
        highScore.SetActive(true);
    }
    
    public void GameMenuPhase()
    {
        playButton.SetActive(true);
        highScore.SetActive(false);
    }
}
