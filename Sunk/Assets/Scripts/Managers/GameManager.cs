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

public class GameManager : Singelton<GameManager>
{
    [SerializeField] private int maxHealth;
    [HideInInspector] public int CurrentHealth;

    [HideInInspector] public UnityEvent OnDamageTaken = new UnityEvent();

    private int currentScore = 0;
    private bool hasScoredThisTurn = false;

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
            case FoulType.CueballSunk:
                break;
            case FoulType.Ball8SunkEarly: OnGameOver(); break;
        }
    }

    private void StartGame()
    {
        CurrentHealth = maxHealth;
        BallManager.Instance.InitalizeBalls();
    }

    private void OnBallFired()
    {
        PlayerManager.Instance.DeactivatePlayer();
    }

    private void OnAllBallsStopped()
    {
        if (!hasScoredThisTurn)
        {
            AddFoul(FoulType.NoBallsSunk);
        }

        hasScoredThisTurn = false;

        PlayerManager.Instance.ActivatePlayer();
    }

    private void OnBallDestroyed(BallObject ballObject)
    {
        switch (ballObject.BallData.BallType)
        {
            case BallType.Cue:
                AddFoul(FoulType.CueballSunk); break;
            case BallType.Ball8:
                AddFoul(FoulType.Ball8SunkEarly); break;
            default:
            {
                hasScoredThisTurn = true;
                currentScore++;
            }break;
        }
    }

    private void OnTakeDamage()
    {
        CurrentHealth--;
        OnDamageTaken.Invoke();
        if (CurrentHealth <= 0)
        {
            OnGameOver();
        }
    }

    private void OnGameOver()
    {
        EditorApplication.isPlaying = false;
    }
}
