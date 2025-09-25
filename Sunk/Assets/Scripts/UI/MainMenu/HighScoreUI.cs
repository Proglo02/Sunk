using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    public void SetHighScore(int score)
    {
        scoreText.text = score.ToString();
    }
}
