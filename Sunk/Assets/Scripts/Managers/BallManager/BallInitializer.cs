using UnityEngine;

public class BallInitializer : MonoBehaviour
{
    [Header("Ball Prefabs")]
    [SerializeField] private GameObject ballObject;
    [SerializeField] private GameObject cueBallObject;

    public void InitializeBall(BallLayoutData data)
    {
        switch(data.Ball.Data.BallType)
        {
            case BallType.Cue:
            {
                CueBallObject cueBall = Instantiate(cueBallObject, data.Position, data.Rotation).GetComponent<CueBallObject>();
                cueBall.BallData = data.Ball.Data;
                cueBall.SetMaterial(data.Ball.Data.Material);
                cueBall.transform.parent = transform;
                BallManager.Instance.AddBall(cueBall);
                BallManager.Instance.AddCueBall(cueBall);
            }break;
            default:
            {
                BallObject newBall = Instantiate(ballObject, data.Position, data.Rotation).GetComponent<BallObject>();
                newBall.BallData = data.Ball.Data;
                newBall.SetMaterial(data.Ball.Data.Material);
                newBall.transform.parent = transform;
                BallManager.Instance.AddBall(newBall);
            }break;

        }
    }
}
