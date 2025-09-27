using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BallInitializer))]
public class BallManager : Singleton<BallManager>
{
    [SerializeField] private BallLayout ballLayout;
    [SerializeField] private GameObject confettiPrefab;

    [HideInInspector] public UnityEvent OnAllBallsStopped = new UnityEvent();
    [HideInInspector] public UnityEvent<BallObject> OnAllBallDestroyed = new UnityEvent<BallObject>();
    public CueBallObject CueBall { get; private set; }

    private BallInitializer ballInitializer;
    private List<BallObject> ballObjects = new List<BallObject>();
    private Coroutine ballMovementCheckCoroutine;

    protected override void Awake()
    {
        base.Awake();
        GetComponents();
    }

    /// <summary>
    /// Initializes all balls from the set layout
    /// </summary>
    public void InitalizeBalls()
    {
        foreach (var layoutData in ballLayout.Balls)
        {
            ballInitializer.InitializeBall(layoutData);
        }
    }

    /// <summary>
    /// Adds a ball to the list of managed balls
    /// </summary>
    public void AddBall(BallObject ball)
    {
        if (!ballObjects.Contains(ball))
        {
            ballObjects.Add(ball);
        }
    }

    /// <summary>
    /// Set the cue ball reference
    /// </summary>
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

    public float GetFastestBallSpeed()
    {
        float fastestSpeed = 0f;
        foreach (var ball in ballObjects)
        {
            float velocity = ball.GetComponent<Rigidbody>().velocity.magnitude;
            float angularVelocity = ball.GetComponent<Rigidbody>().angularVelocity.magnitude;

            if (ball.IsMoving && (velocity > fastestSpeed || angularVelocity > fastestSpeed))
            {
                fastestSpeed = Mathf.Max(velocity, angularVelocity);
            }
        }
        return fastestSpeed;
    }

    public bool IsAnyBallMoving()
    {
        foreach (var ball in ballObjects)
        {
            if (ball.IsMoving)
                return true;
        }

        return false;
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

    private void GetComponents()
    {
        ballInitializer = GetComponent<BallInitializer>();
    }

    private void OnBallFired()
    {
        if(ballMovementCheckCoroutine == null)
            ballMovementCheckCoroutine = StartCoroutine(BallMovementCheck());
    }

    private IEnumerator BallMovementCheck()
    {
        yield return new WaitForSeconds(.1f);

        while (IsAnyBallMoving())
        {
            yield return new WaitForEndOfFrame();
        }

        ballMovementCheckCoroutine = null;
        OnAllBallsStopped.Invoke();
    }
}
