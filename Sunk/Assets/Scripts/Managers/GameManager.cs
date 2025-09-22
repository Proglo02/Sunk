using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BallInitializer))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private BallLayout ballLayout;

    private BallInitializer ballInitializer;

    private void Awake()
    {
        GetComponents();
    }

    private void Start()
    {
        StartGame();
    }

    private void GetComponents()
    {
        ballInitializer = GetComponent<BallInitializer>();
    }

    private void StartGame()
    {
        InitalizeBalls();
    }

    private void InitalizeBalls()
    {
        foreach(var layoutData in ballLayout.Balls)
        {
            ballInitializer.InitializeBall(layoutData);
        }
    }
}
