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
}
