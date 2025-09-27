using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum FoulType
{
    NoBallsSunk,
    CueballSunk,
    Ball8SunkEarly,
}

public abstract class GameManager<T> : GameManager where T : GameManager<T>
{
    [HideInInspector] public static new T Instance
        => GameManager.Instance as T;
}

public abstract class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameOverMenu gameOverMenu;

    [HideInInspector] public int CurrentRound = 1;
    public bool isGameOver { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();

        GetComponents();
        BindEvents();

        StartCoroutine(BindCueBallEvents());
    }

    private void Start()
    {
        StartGame();
    }

    /// <summary>
    /// Applies the effects of a foul based on its type
    /// </summary>
    public virtual void AddFoul(FoulType foulType)
    {

    }

    public void StartFastForward()
    {
        Time.timeScale = 3;
    }

    public void StopFastForward()
    {
        Time.timeScale = 1f;
    }

    protected virtual void StartGame()
    {
        BallManager.Instance.InitalizeBalls();
        PlayerManager.Instance.ActivatePlayer();
    }

    protected void EndGame(bool playerWon)
    {
        if(isGameOver)
            return;

        isGameOver = true;

        if (playerWon)
            SaveSystem.SaveData();

        gameOverMenu.Activate(playerWon);
    }

    protected virtual void OnAllBallsStopped() { }
    protected virtual void OnBallSunk(BallObject ballObject, Pocket pocket) { }
    protected virtual void OnRoundOver()
    {
        StopFastForward();
    }

    private void GetComponents()
    {

    }

    private void BindEvents()
    {
        BallManager.Instance.OnAllBallsStopped.AddListener(OnAllBallsStopped);
        BallManager.Instance.OnBallSunk.AddListener(OnBallSunk);
    }

    private void OnBallFired()
    {
        PlayerManager.Instance.DeactivatePlayer();
    }

    private IEnumerator BindCueBallEvents()
    {
        while (!BallManager.Instance.CueBall)
            yield return new WaitForEndOfFrame();

        BallManager.Instance.CueBall.OnBallFired.AddListener(OnBallFired);
    }
}
