using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;

public class CueBallObject : BallObject
{
    [SerializeField] private GameObject aimTarget;
    [Header("Shoot Force")]
    [SerializeField] private float maxShootForce;
    public float MaxShootForce => maxShootForce;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float minShootForce;

    [HideInInspector] public UnityEvent OnBallFired = new UnityEvent();
    [HideInInspector] public UnityEvent OnBallCharging = new UnityEvent();

    private int aimAngle = 180;
    public bool IsCharging { get; private set; } = false;
    public float ShootForce { get; private set; } = 0f;

    private bool isDisabled = false;
    private Vector3 lastSafeLocation;

    protected override void Awake()
    {
        base.Awake();
        BindEvents();
    }

    private void Start()
    {
        SetAimTargetPosition();
        lastSafeLocation = transform.position;
    }

    private void BindEvents()
    {
        BallManager.Instance.OnAllBallsStopped.AddListener(OnAllBallsStopped);
    }

    private void OnAllBallsStopped()
    {
        if (isDisabled)
        {
            transform.position = lastSafeLocation;
            gameObject.SetActive(true);
            isDisabled = false;
        }
        else
            lastSafeLocation = transform.position;

        aimTarget.SetActive(true);
        SetAimTargetPosition();
    }

    public void FireBall()
    {
        if(!IsCharging)
        {
            StartCoroutine(ChargeShootForce());
            return;
        }

        OnBallFired.Invoke();

        aimTarget.SetActive(false);

        IsCharging = false;

        Vector3 force = Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward * ShootForce;
        force.y = 0;
        AddForce(force);
    }

    public void SetAimDegree(InputAction.CallbackContext context)
    {
        if (IsCharging)
            return;

        int value  = context.ReadValue<float>() > 0 ? 1 : -1;
        SetAimAngle(aimAngle + value);
    }

    public void SetAimCardinal(InputAction.CallbackContext context)
    {
        if (IsCharging)
            return;

        int value = context.ReadValue<float>() > 0 ? 1 : -1;
        int angle = aimAngle;
        angle /= 45;
        angle = aimAngle % 45 != 0 ? angle + (int)((value + 1) * .5f) : angle + value;
        angle *= 45;

        SetAimAngle(angle);
    }

    private void SetAimTargetPosition()
    {
        Vector3 direction = Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;

        Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, 20f);

        if (hit.collider != null)
        {
            aimTarget.transform.position = transform.position + direction * (hit.distance - .2f) + new Vector3(0f, -.4f, 0f);
        }

        aimTarget.transform.rotation = Quaternion.Euler(new Vector3(90, 90, 0));
    }

    private IEnumerator ChargeShootForce()
    {
        IsCharging = true;
        OnBallCharging.Invoke();

        int chargeDirection = 1;

        while(IsCharging)
        {
            ShootForce += Time.deltaTime * chargeDirection * chargeSpeed;

            if (ShootForce > 10 || ShootForce < 0)
                chargeDirection *= -1;

            ShootForce = Mathf.Clamp(ShootForce, 0, 10);

            yield return new WaitForEndOfFrame();
        }

        ShootForce = 0;
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

        SetAimTargetPosition();
    }

    public void Disable()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        rigidBody.Sleep();
        gameObject.SetActive(false);
        IsMoving = false;

        isDisabled = true;
    }
}
