using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;

    public void Activate(bool won)
    {
        if (won)
            titleText.text = "You Won!" + "\nShots Taken: " + GameManager.Instance.CurrentRound;
        else
            titleText.text = "Game Over!";

        gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DefaultGameScene");
    }

    public void QuitToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
