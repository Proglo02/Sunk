using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;
using System;

public class CueBallObject : BallObject
{
    [SerializeField] private AimTarget aimTarget;
    [Header("Shoot Force")]
    [SerializeField] private float maxShootForce;
    public float MaxShootForce => maxShootForce;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float minShootForce;

    [Header("Aim Rotation")]
    [SerializeField] private float rotationSpeedDegree = 1;
    [SerializeField] private float rotationSpeedCardinal = 1;

    [HideInInspector] public UnityEvent OnBallFired = new UnityEvent();
    [HideInInspector] public UnityEvent OnBallCharging = new UnityEvent();

    public float ShootForce { get; private set; } = 0f;
    public bool IsCharging { get; private set; } = false;
    public Vector3 LastSafeLocation { get; private set; }

    private float aimAngle = 180;

    private Coroutine degreeRotation;
    private Coroutine cardinalRotation;

    protected override void Awake()
    {
        base.Awake();
        BindEvents();
    }

    private void Start()
    {
        SetAimTargetPosition();
        aimTarget.SetLine();
        LastSafeLocation = transform.position;
    }

    private void OnDestroy()
    {
        OnBallFired.RemoveAllListeners();
        OnBallCharging.RemoveAllListeners();
    }

    /// <summary>
    /// Fires the ball in the aimed direction with the charged force <br/>
    /// If not charging, starts charging the ball
    /// </summary>
    public void FireBall()
    {
        //If not charging, start charging the ball
        if (!IsCharging)
        {
            StartCoroutine(ChargeShootForce());
            return;
        }

        OnBallFired.Invoke();

        aimTarget.gameObject.SetActive(false);
        IsCharging = false;

        Vector3 force = Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward * ShootForce;
        force.y = 0;
        AddForce(force);
    }

    /// <summary>
    /// Rotates the aim angle by 1 degree
    /// </summary>
    public void SetAimDegree(InputAction.CallbackContext context)
    {
        if (IsCharging)
            return;

        float value = context.ReadValue<float>() > 0 ? .5f : -.5f;
        SetAimAngle(aimAngle + value);
    }

    /// <summary>
    /// Rotates the aim angle to the closest cardinal direction (0, 45, 90, etc)
    /// </summary>
    public void SetAimCardinal(InputAction.CallbackContext context)
    {
        if (IsCharging)
            return;

        int value = context.ReadValue<float>() > 0 ? 1 : -1;
        int angle = (int)aimAngle;
        angle /= 45;
        angle = aimAngle % 45 != 0 ? angle + (int)((value + 1) * .5f) : angle + value;
        angle *= 45;

        SetAimAngle(angle);
    }

    /// <summary>
    /// Rotates the aim angle by 1 degree over time
    /// </summary>
    public void DoAimDegreeRotate(InputAction.CallbackContext context)
    {
        if (IsCharging)
            return;

        if (degreeRotation == null)
            degreeRotation = StartCoroutine(RotateDegree(context));
    }

    /// <summary>
    /// Rotates the aim angle to the closest cardinal direction (0, 45, 90, etc) over time
    /// </summary>
    /// <param name="context"></param>
    public void DoAimCardinalRotation(InputAction.CallbackContext context)
    {
        if (IsCharging)
            return;

        if (cardinalRotation == null)
            cardinalRotation = StartCoroutine(RotateCardinal(context));
    }

    /// <summary>
    /// Stops any on going rotation of the aim angle
    /// </summary>
    public void CancelRotate()
    {
        if (degreeRotation != null)
        {
            StopCoroutine(degreeRotation);
            degreeRotation = null;
        }

        if(cardinalRotation != null)
        {
            StopCoroutine(cardinalRotation);
            cardinalRotation = null;
        }
    }

    private void BindEvents()
    {
        BallManager.Instance.OnAllBallsStopped.AddListener(OnAllBallsStopped);
    }

    private void OnAllBallsStopped()
    {
        if(GameManager.Instance.isGameOver)
            return;

        LastSafeLocation = transform.position;

        aimTarget.gameObject.SetActive(true);
        SetAimTargetPosition();
        aimTarget.SetLine();
    }

    private void SetAimTargetPosition()
    {
        Vector3 direction = Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;

        Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, 20f);

        if (hit.collider != null)
        {
            aimTarget.transform.position = transform.position + direction * (hit.distance - .2f) + new Vector3(0f, -.3f, 0f);
        }

        aimTarget.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void AddForce(Vector3 force)
    {
        rigidBody.AddForce(force, ForceMode.Impulse);
    }

    private void SetAimAngle(float angle)
    {
        if (angle > 359f)
            aimAngle = angle % 360f;
        else if (angle < 0f)
            aimAngle = (angle % 360f) + 360f;
        else
            aimAngle = angle;

        SetAimTargetPosition();
        aimTarget.SetLine();
    }

    private IEnumerator ChargeShootForce()
    {
        IsCharging = true;
        OnBallCharging.Invoke();

        int chargeDirection = 1;

        while (IsCharging)
        {
            ShootForce += Time.deltaTime * chargeDirection * chargeSpeed;

            if (ShootForce > 10 || ShootForce < 0)
                chargeDirection *= -1;

            ShootForce = Mathf.Clamp(ShootForce, 0, 10);

            yield return new WaitForEndOfFrame();
        }

        ShootForce = 0;
    }

    private IEnumerator RotateDegree(InputAction.CallbackContext context)
    {
        while(true)
        {
            SetAimDegree(context);

            yield return new WaitForSeconds(1 / rotationSpeedDegree);
        }
    }

    private IEnumerator RotateCardinal(InputAction.CallbackContext context)
    {
        while (true)
        {
            SetAimCardinal(context);

            yield return new WaitForSeconds(1 / rotationSpeedCardinal);
        }
    }
}
