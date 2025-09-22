using UnityEngine;

public class CueBall : BallObject
{
    private Rigidbody rigidbody;

    private void Awake()
    {
        GetComponents();
    }

    private void GetComponents()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void FireBall()
    {
         Vector3 force = transform.forward * -5f;
        AddForce(force);
    }

    public void AddForce(Vector3 force)
    {
         rigidbody.AddForce(force, ForceMode.Impulse);
    }
}
