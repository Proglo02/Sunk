using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class CueBallObject : BallObject
{
    [SerializeField] private GameObject aimTarget;

    private int aimAngle = 0;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        SetAimTargetPosition();
    }

    public void FireBall()
    {
        if (!GameManager.Instance.PlayerIsActive)
            return;

        //TODO: Do shoot event
        GameManager.Instance.PlayerIsActive = false;

        aimTarget.SetActive(false);

        Vector3 force = Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward * 5f;
        force.y = 0;
        AddForce(force);

        IsMoving = true;

        StartCoroutine(BallManager.Instance.BallMovementCheck());
    }

    public void SetAimDegree(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        int rawValue = (int)context.ReadValue<float>();
        SetAimAngle(aimAngle + rawValue);
    }

    public void SetAimCardinal(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        int rawValue = (int)context.ReadValue<float>();
        int angle = aimAngle;
        angle /= 45;
        angle = aimAngle % 45 != 0 ? angle + (int)((rawValue + 1) * .5f) : angle + rawValue;
        angle *= 45;

        SetAimAngle(angle);
    }

    private void SetAimTargetPosition()
    {
        if (GameManager.Instance.PlayerIsActive)
        {
            if (!aimTarget.activeSelf)
                aimTarget.SetActive(true);

            Vector3 direction = Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;

            Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, 20f);

            if (hit.collider != null)
            {
                aimTarget.transform.position = transform.position + direction * (hit.distance - .2f) + new Vector3(0f, -.4f, 0f);
                Debug.DrawLine(transform.position, transform.position + direction * (hit.distance - .2f));
            }

            aimTarget.transform.rotation = Quaternion.Euler(new Vector3(90, 90, 0));
        }
    }

    private void AddForce(Vector3 force)
    {
        rigidBody.AddForce(force, ForceMode.Impulse);
    }

    private void SetAimAngle(int angle)
    {
        if (angle > 359)
            aimAngle = angle % 360;
        else if (angle < 0)
            aimAngle = (angle % 360) + 360;
        else
            aimAngle = angle;
    }
}
