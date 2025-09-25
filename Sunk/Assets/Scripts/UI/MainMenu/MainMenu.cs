using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private HighScoreUI highScoreUI;

    private void Awake()
    {
        int highscore = SaveSystem.LoadData().HighScore;

        if (highscore > 0)
            highScoreUI.SetHighScore(highscore);
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DefaultGameScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
