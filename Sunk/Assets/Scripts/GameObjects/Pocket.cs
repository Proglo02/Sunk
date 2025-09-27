using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pocket : DestroyBallTrigger
{
    protected override void DestroyBall(GameObject ball)
    {
        BallManager.Instance.SinkBall(ball, this);
    }
}
