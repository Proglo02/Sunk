using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private GameObject liveIconPrefab;
    [SerializeField] private GameObject lives;

    private void Awake()
    {
        InstantiateLiveIcons();
        StandardBilliardsManager.Instance.OnDamageTaken.AddListener(OnDamageTaken);
        StandardBilliardsManager.Instance.OnHealthAdded.AddListener(OnHealthAdded);

    }
    private void InstantiateLiveIcons()
    {
        for (int i = 0; i < StandardBilliardsManager.Instance.MaxHealth; i++)
        {
            Instantiate(liveIconPrefab, lives.transform);
        }
    }

    private void OnDamageTaken()
    {
        Destroy(lives.transform.GetChild(0).gameObject);
    }

    private void OnHealthAdded()
    {
        Instantiate(liveIconPrefab, lives.transform);
    }
}
