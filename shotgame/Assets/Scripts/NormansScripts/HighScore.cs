using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    private int killScore = 0;
    [SerializeField] string textBeforeScore;
    private TextMeshProUGUI tmp;

    private void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        tmp.text = textBeforeScore + killScore.ToString();
    }

    public void AddScore()
    {
        killScore++;
    }
}
