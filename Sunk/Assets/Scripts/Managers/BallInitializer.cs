using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallInitializer : MonoBehaviour
{
    [SerializeField] private GameObject ballObject;
    [SerializeField] private GameObject cueBallObject;

    public void InitializeBall(BallLayoutData data)
    {
        if(data.Ball.Data.Number == 0)
        {
            BallObject cueBall = Instantiate(cueBallObject, data.Position, data.Rotation).GetComponent<BallObject>();
            cueBall.BallData = data.Ball.Data;
            cueBall.SetMaterial(data.Ball.Data.Material);
            cueBall.transform.parent = transform;
            BallManager.Instance.AddBall(cueBall);
            return;
        }

        BallObject newBall = Instantiate(ballObject, data.Position, data.Rotation).GetComponent<BallObject>();
        newBall.BallData = data.Ball.Data;
        newBall.SetMaterial(data.Ball.Data.Material);
        newBall.transform.parent = transform;
        BallManager.Instance.AddBall(newBall);
    }
}
