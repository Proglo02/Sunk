using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Player : MonoBehaviour
{
    private BallType ballType;
    private bool isActive = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Assigns the ball type to the player <br/>
    /// Can Only be Solid or Striped
    /// </summary>
    public void AssignBallType(BallType ballType)
    {
        if (ballType != BallType.Solid && ballType != BallType.Striped)
            return;

        this.ballType = ballType;
    }

    /// <summary>
    /// Fires the cue ball if the player is active
    /// </summary>
    public void OnFire(InputAction.CallbackContext context)
    {
        if (!context.started || !isActive)
            return;

        if(BallManager.Instance)
            BallManager.Instance.CueBall.FireBall();
    }

    /// <summary>
    /// Rotates the aim of the cue ball by 1 degree if the player is active
    /// </summary>
    public void OnRotateStep(InputAction.CallbackContext context)
    {
        if (!isActive)
            return;

        if (context.started)
            BallManager.Instance.CueBall.SetAimDegree(context);
        else if (context.canceled)
            BallManager.Instance.CueBall.CancelRotate();
        else if (context.interaction is HoldInteraction)
            BallManager.Instance.CueBall.DoAimDegreeRotate(context);
    }

    /// <summary>
    /// Rotates the aim of the cue ball to the closest cardinal direction (0, 45, 90, etc) if the player is active
    /// </summary>
    public void OnRotateCardinal(InputAction.CallbackContext context)
    {
        if (!isActive)
            return;

        if (context.started)
            BallManager.Instance.CueBall.SetAimCardinal(context);
        else if (context.canceled)
            BallManager.Instance.CueBall.CancelRotate();
        else if (context.interaction is HoldInteraction)
            BallManager.Instance.CueBall.DoAimCardinalRotation(context);
    }

    /// <summary>
    /// Rotates the aim of the cue ball if the player is active <br/>
    /// Checks if the right mouse button is held to determine whether to rotate by step or cardinal
    /// </summary>
    public void OnRotateScroll(InputAction.CallbackContext context)
    {
        if (!context.started || !isActive)
            return;

        if (Mouse.current.rightButton.isPressed)
            OnRotateCardinal(context);
        else
            OnRotateStep(context);
    }

    /// <summary>
    /// Increases the time scale to 5x
    /// </summary>
    public void FastForward(InputAction.CallbackContext context)
    {
        float speed = BallManager.Instance.GetFastestBallSpeed();

        if (context.started && speed < 0f && BallManager.Instance.IsAnyBallMoving())
            GameManager.Instance.StartFastForward();
        else if (context.canceled)
            GameManager.Instance.StopFastForward();
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }
}
