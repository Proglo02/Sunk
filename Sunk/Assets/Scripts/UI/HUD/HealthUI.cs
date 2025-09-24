using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private GameObject lives;

    private void Awake()
    {
        GameManager.Instance.OnDamageTaken.AddListener(OnDamageTaken);
    }

    private void OnDamageTaken()
    {
        Destroy(lives.transform.GetChild(0).gameObject);
    }
}
