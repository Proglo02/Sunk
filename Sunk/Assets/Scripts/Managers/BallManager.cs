using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GridLayout;

[RequireComponent(typeof(BallInitializer))]
public class BallManager : Singelton<BallManager>
{
    [SerializeField] private BallLayout ballLayout;

    private BallInitializer ballInitializer;

    public UnityEvent OnAllBallsStopped = new UnityEvent();

    private List<BallObject> ballObjects = new List<BallObject>();

    protected override void Awake()
    {
        base.Awake();
        GetComponents();
    }

    private void GetComponents()
    {
        ballInitializer = GetComponent<BallInitializer>();
    }

    public void InitalizeBalls()
    {
        foreach (var layoutData in ballLayout.Balls)
        {
            ballInitializer.InitializeBall(layoutData);
        }
    }

    public void AddBall(BallObject ball)
    {
        if (!ballObjects.Contains(ball))
        {
            ballObjects.Add(ball);
        }
    }

    public void DestroyBall(GameObject ball)
    {
        BallObject ballObject = ball.GetComponent<BallObject>();

        if (!ballObject)
            return;

        if (ballObjects.Contains(ballObject))
            ballObjects.Remove(ballObject);

        Destroy(ball);
    }

    public IEnumerator BallMovementCheck()
    {
        yield return new WaitForSeconds(1);

        while (!AllBallsHaveStopped())
        {
            yield return new WaitForEndOfFrame();
        }
        OnAllBallsStopped.Invoke();
    }

    private bool AllBallsHaveStopped()
    {
        foreach(var ball in ballObjects)
        {
            if (ball.IsMoving)
                return false;
        }

        return true;
    }
}
