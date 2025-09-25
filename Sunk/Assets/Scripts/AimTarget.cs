using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTarget : MonoBehaviour
{
    private Material lineMaterial;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineMaterial = GetComponent<Renderer>().material;
    }

    public void SetLine()
    {
        lineRenderer.positionCount = 2;

        Vector3 point1 = transform.parent.position + new Vector3(0f, -.29f, 0f);
        Vector3 point2 = transform.position + new Vector3(0f, .01f, 0f);

        float distance = Vector3.Distance(point1, point2);

        lineMaterial.SetFloat("_Tiling", distance * .47f);

        lineRenderer.SetPosition(0, point1);
        lineRenderer.SetPosition(1, point2);
    }
}
