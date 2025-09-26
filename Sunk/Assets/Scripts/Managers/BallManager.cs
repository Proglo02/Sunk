using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BallInitializer))]
public class BallManager : Singleton<BallManager>
{
    [SerializeField] private BallLayout ballLayout;

    [SerializeField] private GameObject confettiPrefab;

    private BallInitializer ballInitializer;

    [HideInInspector] public UnityEvent OnAllBallsStopped = new UnityEvent();
    [HideInInspector] public UnityEvent<BallObject> OnAllBallDestroyed = new UnityEvent<BallObject>();

    private List<BallObject> ballObjects = new List<BallObject>();
    public CueBallObject CueBall { get; private set; }

    private Coroutine ballMovementCheckCoroutine;

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

    public void AddCueBall(CueBallObject cueBall)
    {
        if(!CueBall)
        {
            CueBall = cueBall;
            CueBall.OnBallFired.AddListener(OnBallFired);
        }
    }

    public int GetNumBalls()
    {
        return ballObjects.Count;
    }

    public void DestroyBall(GameObject ball)
    {
        BallObject ballObject = ball.GetComponent<BallObject>();

        if (!ballObject)
            return;

        if (confettiPrefab)
            Instantiate(confettiPrefab, ball.transform.position, Quaternion.Euler(-90f, 0f, 0f));

        if (ballObject.BallData.BallType == BallType.Cue)
        {
            CueBall.Disable();
            OnAllBallDestroyed.Invoke(ballObject);
            return;
        }

        if (ballObjects.Contains(ballObject))
        {
            ballObjects.Remove(ballObject);
            OnAllBallDestroyed.Invoke(ballObject);
        }

        Destroy(ball);
    }

    public IEnumerator BallMovementCheck()
    {
        yield return new WaitForSeconds(.1f);

        while (!AllBallsHaveStopped())
        {
            yield return new WaitForEndOfFrame();
        }

        ballMovementCheckCoroutine = null;
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

    private void OnBallFired()
    {
        if(ballMovementCheckCoroutine == null)
            ballMovementCheckCoroutine = StartCoroutine(BallMovementCheck());
    }
}
