using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private BallType ballType;
    private bool isActive = false;

    public void AssignBallType(BallType ballType)
    {
        if (ballType != BallType.Solid && ballType != BallType.Striped)
            return;

        this.ballType = ballType;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!context.started || !isActive)
            return;

        BallManager.Instance.CueBall.FireBall();
    }

    public void OnRotateStep(InputAction.CallbackContext context)
    {
        if (!context.started || !isActive)
            return;

        BallManager.Instance.CueBall.SetAimDegree(context);
    }

    public void OnRotateCardinal(InputAction.CallbackContext context)
    {
        if (!context.started || !isActive)
            return;

        BallManager.Instance.CueBall.SetAimCardinal(context);
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }
}
