using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeMeter : MonoBehaviour
{
    [SerializeField] private GameObject meter;
    [SerializeField] private Image fill;

    private void Awake()
    {
        StartCoroutine(BindCueBallEvents());
    }

    private IEnumerator BindCueBallEvents()
    {
        while (!BallManager.Instance.CueBall)
            yield return new WaitForEndOfFrame();

        BallManager.Instance.CueBall.OnBallFired.AddListener(OnBallFired);
        BallManager.Instance.CueBall.OnBallCharging.AddListener(OnBallCharging);
    }

    private void Update()
    {
        SetFill();
    }

    private void SetFill()
    {
        if (!BallManager.Instance.CueBall || !BallManager.Instance.CueBall.IsCharging)
            return;

        CueBallObject cueBall = BallManager.Instance.CueBall;

        float percentCharge = cueBall.ShootForce / cueBall.MaxShootForce;

        fill.rectTransform.sizeDelta = new Vector2(100, 350 * percentCharge);
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
