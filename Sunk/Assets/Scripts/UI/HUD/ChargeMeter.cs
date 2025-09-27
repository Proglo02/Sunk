using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChargeMeter : MonoBehaviour
{
    [SerializeField] private GameObject meter;
    [SerializeField] private Image fill;

    private void Awake()
    {
        BindEvents();
    }

    private void Update()
    {
        SetFill();
    }

    private void BindEvents()
    {
        BallManager.Instance.OnCueBallAdded.AddListener(OnCueBallAdded);
    }

    private void SetFill()
    {
        if (!BallManager.Instance.CueBall || !BallManager.Instance.CueBall.IsCharging)
            return;

        CueBallObject cueBall = BallManager.Instance.CueBall;

        float percentCharge = cueBall.ShootForce / cueBall.MaxShootForce;

        fill.rectTransform.sizeDelta = new Vector2(100, 350 * percentCharge);
    }

    private void OnCueBallAdded()
    {
        BallManager.Instance.CueBall.OnBallFired.AddListener(OnBallFired);
        BallManager.Instance.CueBall.OnBallCharging.AddListener(OnBallCharging);
    }

    private void OnBallFired()
    {
        meter.SetActive(false);
    }

    private void OnBallCharging()
    {
        meter.SetActive(true);
    }
}
