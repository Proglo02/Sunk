using UnityEngine;

public class BallInitializer : MonoBehaviour
{
    [Header("Ball Prefabs")]
    [SerializeField] private GameObject ballObject;
    [SerializeField] private GameObject cueBallObject;

    public void InstantiateBall(BallLayoutData data)
    {
        InstantiateBall(data.Ball.Data, data.Position, data.Rotation);
    }

    public void InstantiateBall(BallData data, Vector3 position, Quaternion rotation)
    {
        switch (data.BallType)
        {
            case BallType.Cue:
                {
                    CueBallObject cueBall = Instantiate(cueBallObject, position, rotation).GetComponent<CueBallObject>();
                    cueBall.BallData = data;
                    cueBall.SetMaterial(data.Material);
                    cueBall.transform.parent = transform;
                    BallManager.Instance.AddBall(cueBall);
                    BallManager.Instance.AddCueBall(cueBall);
                }
                break;
            default:
                {
                    BallObject newBall = Instantiate(ballObject, position, rotation).GetComponent<BallObject>();
                    newBall.BallData = data;
                    newBall.SetMaterial(data.Material);
                    newBall.transform.parent = transform;
                    BallManager.Instance.AddBall(newBall);
                }
                break;
        }
    }
}
