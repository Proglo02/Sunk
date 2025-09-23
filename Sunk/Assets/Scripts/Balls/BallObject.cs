using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(Rigidbody))]
public class BallObject : MonoBehaviour
{
    public BallData BallData;
    public bool IsMoving { get; protected set; } = false;

    protected Rigidbody rigidBody;

    private void Awake()
    {
        GetComponents();
    }

    protected virtual void FixedUpdate()
    {
        DoGroundCheck();
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

    private void DoGroundCheck()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.6f);

        if (hit.collider != null && hit.collider.CompareTag("Table"))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.4f, transform.position.z);
        }
    }

    private void DoMotionCheck()
    {
        IsMoving = rigidBody.velocity.magnitude > .135f;
    }
}
