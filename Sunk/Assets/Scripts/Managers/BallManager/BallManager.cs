using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BallInitializer))]
public class BallManager : Singleton<BallManager>
{
    [SerializeField] private BallLayout ballLayout;
    [SerializeField] private GameObject confettiPrefab;

    [HideInInspector] public UnityEvent OnAllBallsStopped               = new UnityEvent();
    [HideInInspector] public UnityEvent OnCueBallAdded                  = new UnityEvent();
    [HideInInspector] public UnityEvent<BallObject> OnBallDestroyed     = new UnityEvent<BallObject>();
    [HideInInspector] public UnityEvent<BallObject, Pocket> OnBallSunk  = new UnityEvent<BallObject, Pocket>();
    public CueBallObject CueBall { get; private set; }

    private BallInitializer ballInitializer;
    private List<BallObject> ballObjects = new List<BallObject>();
    private Coroutine ballMovementCheckCoroutine;

    private BallData savedCueBallData;
    private Vector3 lastSafeCueBallPosition;

    protected override void Awake()
    {
        base.Awake();
        GetComponents();
    }

    /// <summary>
    /// Initializes all balls from the set layout
    /// </summary>
    public void InstantiateBalls()
    {
        foreach (var layoutData in ballLayout.Balls)
        {
            ballInitializer.InstantiateBall(layoutData);
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
            savedCueBallData = CueBall.BallData;
            CueBall.OnBallFired.AddListener(OnBallFired);
            OnCueBallAdded.Invoke();
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

        if (ballObject.BallData.BallType == BallType.Cue)
        {
            lastSafeCueBallPosition = CueBall.LastSafeLocation;
            StartCoroutine(ResetCueball());
        }

        if (ballObjects.Contains(ballObject))
        {
            ballObjects.Remove(ballObject);
            OnBallDestroyed.Invoke(ballObject);
        }

        Destroy(ball);
    }

    public void SinkBall(GameObject ball, Pocket pocket)
    {
        BallObject ballObject = ball.GetComponent<BallObject>();

        if (!ballObject)
            return;

        if (confettiPrefab)
            Instantiate(confettiPrefab, ball.transform.position, Quaternion.Euler(-90f, 0f, 0f));

        OnBallSunk.Invoke(ballObject, pocket);

        DestroyBall(ball);
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

    private void InstantiateCueBall()
    {
        Collider[] colliders = Physics.OverlapSphere(lastSafeCueBallPosition, .3f);

        bool overlapingBall = false;

        foreach(var collider in colliders)
        {
            if(collider.CompareTag("Ball"))
            {
                overlapingBall = true;
                break;
            }
        }

        if (overlapingBall)
            lastSafeCueBallPosition += new Vector3(0f, 1f, 0f);

        ballInitializer.InstantiateBall(savedCueBallData, lastSafeCueBallPosition, Quaternion.identity);
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

    private IEnumerator ResetCueball()
    {
        if (!GameManager.Instance.isGameOver)
        {
            while (IsAnyBallMoving())
            {
                yield return new WaitForEndOfFrame();
            }

            InstantiateCueBall();
        }
    }
}
