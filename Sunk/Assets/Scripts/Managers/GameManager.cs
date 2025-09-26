using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum FoulType
{
    NoBallsSunk,
    CueballSunk,
    Ball8SunkEarly,
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameOverMenu gameOverMenu;

    [Header("Game Settings")]
    [SerializeField] public int MaxHealth;
    [HideInInspector] public int CurrentHealth;

    [HideInInspector] public UnityEvent OnDamageTaken = new UnityEvent();
    [HideInInspector] public UnityEvent OnHealthAdded = new UnityEvent();

    [HideInInspector] public int CurrentRound = 1;

    private int currentScore = 0;
    private bool hasScoredThisTurn = false;
    private bool hasTakenDamageThisTurn = false;
    private bool shouldAddHealth = false;
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

    private void GetComponents()
    {

    }

    private void BindEvents()
    {
        BallManager.Instance.OnAllBallsStopped.AddListener(OnAllBallsStopped);
        BallManager.Instance.OnAllBallDestroyed.AddListener(OnBallDestroyed);
    }

    private IEnumerator BindCueBallEvents()
    {
        while (!BallManager.Instance.CueBall)
            yield return new WaitForEndOfFrame();

        BallManager.Instance.CueBall.OnBallFired.AddListener(OnBallFired);
    }

    public void AddFoul(FoulType foulType)
    {
        switch (foulType)
        {
            case FoulType.NoBallsSunk: OnTakeDamage(); break;
            case FoulType.CueballSunk: OnTakeDamage(); break;
            case FoulType.Ball8SunkEarly: EndGame(false); break;
        }
    }

    private void StartGame()
    {
        CurrentHealth = MaxHealth;
        BallManager.Instance.InitalizeBalls();
        PlayerManager.Instance.ActivatePlayer();
    }

    private void OnBallFired()
    {
        PlayerManager.Instance.DeactivatePlayer();
    }

    private void OnAllBallsStopped()
    {
        if (CurrentRound > 1 && !hasScoredThisTurn)
        {
            AddFoul(FoulType.NoBallsSunk);
        }

        if (BallManager.Instance.GetNumBalls() <= 1)
            EndGame(true);
        else if (!isGameOver)
            OnRoundOver();
    }

    private void OnRoundOver()
    {
        CurrentRound++;

        if (!hasTakenDamageThisTurn && shouldAddHealth)
        {
            CurrentHealth++;
            OnHealthAdded.Invoke();
            shouldAddHealth = false;
        }

        hasScoredThisTurn = false;
        hasTakenDamageThisTurn = false;
        PlayerManager.Instance.ActivatePlayer();
    }

    private void OnBallDestroyed(BallObject ballObject)
    {
        switch (ballObject.BallData.BallType)
        {
            case BallType.Cue:
                AddFoul(FoulType.CueballSunk); break;
            case BallType.Ball8:
            {
                if(BallManager.Instance.GetNumBalls() > 2)
                    AddFoul(FoulType.Ball8SunkEarly);
            }break;
            default:
            {
                if (hasScoredThisTurn)
                    shouldAddHealth = true;

                hasScoredThisTurn = true;
                currentScore++;
            }break;
        }
    }

    private void OnTakeDamage()
    {
        if (hasTakenDamageThisTurn)
            return;

        hasTakenDamageThisTurn = true;

        CurrentHealth--;
        OnDamageTaken.Invoke();
        if (CurrentHealth <= 0)
        {
            EndGame(false);
        }
    }

    private void EndGame(bool playerWon)
    {
        isGameOver = true;

        if(playerWon)
            SaveSystem.SaveData();

        gameOverMenu.Activate(playerWon);
    }
}
