using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(Rigidbody))]
public class BallObject : MonoBehaviour
{
    public BallData BallData;

    public void SetMaterial(Material material)
    {
        GetComponent<MeshRenderer>().material = material;
    }

    protected virtual void FixedUpdate()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.6f);

        if (hit.collider != null && hit.collider.CompareTag("Table"))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.4f, transform.position.z);
        }
    }
}
