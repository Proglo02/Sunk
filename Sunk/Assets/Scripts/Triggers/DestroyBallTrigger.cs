using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DestroyBallTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ball"))
        {
            DestroyBall(other.gameObject);
        }
    }

    protected virtual void DestroyBall(GameObject ball)
    {
        BallManager.Instance.DestroyBall(ball);
    }
}
