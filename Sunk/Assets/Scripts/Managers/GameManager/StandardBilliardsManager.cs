using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StandardBilliardsManager : GameManager<StandardBilliardsManager>
{
    [Header("Game Settings")]
    [SerializeField] public int MaxHealth;

    [HideInInspector] public UnityEvent OnDamageTaken = new UnityEvent();
    [HideInInspector] public UnityEvent OnHealthAdded = new UnityEvent();

    [HideInInspector] public int CurrentHealth;

    private Pocket lastSunkPocket = null;

    private bool hasScoredThisTurn = false;
    private bool hasTakenDamageThisTurn = false;
    private bool shouldAddHealth = false;

    protected override void StartGame()
    {
        CurrentHealth = MaxHealth;

        base.StartGame();
    }

    public override void AddFoul(FoulType foulType)
    {
        switch (foulType)
        {
            case FoulType.NoBallsSunk: OnTakeDamage(); break;
            case FoulType.CueballSunk: OnTakeDamage(); break;
            case FoulType.Ball8SunkEarly: EndGame(false); break;
        }
    }

    protected override void OnAllBallsStopped()
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

    protected override void OnRoundOver()
    {
        base.OnRoundOver();

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

    protected override void OnBallSunk(BallObject ballObject, Pocket pocket)
    {
        switch (ballObject.BallData.BallType)
        {
            case BallType.Cue:
                AddFoul(FoulType.CueballSunk); break;
            case BallType.Ball8:
                {
                    if (BallManager.Instance.GetNumBalls() > 2 || pocket != lastSunkPocket)
                        AddFoul(FoulType.Ball8SunkEarly);
                }
                break;
            default:
                {
                    if (hasScoredThisTurn)
                        shouldAddHealth = true;

                    lastSunkPocket = pocket;
                    hasScoredThisTurn = true;
                }
                break;
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
}
