using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singelton<GameManager>
{
    public bool PlayerIsActive { get; set; } = true;

    protected override void Awake()
    {
        base.Awake();

        GetComponents();
        BindEvents();
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
    }

    private void StartGame()
    {
        BallManager.Instance.InitalizeBalls();
    }

    private void OnAllBallsStopped()
    {
        PlayerIsActive = true;
    }
}
