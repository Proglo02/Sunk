using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(Rigidbody))]
public class BallObject : MonoBehaviour
{
    [SerializeField] private float freezeVelocity = .2f;

    public BallData BallData;
    public bool IsMoving { get; protected set; } = false;

    protected Rigidbody rigidBody;

    protected virtual void Awake()
    {
        GetComponents();
    }

    protected virtual void FixedUpdate()
    {
        if(DoGroundCheck())
            DoMotionCheck();
    }

    public void SetMaterial(Material material)
    {
        GetComponent<MeshRenderer>().material = material;
    }

    private void GetComponents()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private bool DoGroundCheck()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.6f);

        if (hit.collider != null && hit.collider.CompareTag("Table"))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.3f, transform.position.z);
            return true;
        }

        return false;
    }

    private void DoMotionCheck()
    {
        // Check if it's still moving but below the threshold
        if (IsMoving && rigidBody.angularVelocity.magnitude < freezeVelocity && rigidBody.velocity.magnitude < freezeVelocity)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.Sleep();
            IsMoving = false;
        }
        else
            IsMoving = rigidBody.angularVelocity.magnitude > .13f || rigidBody.velocity.magnitude > .2f;
    }
}
