using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererController : MonoBehaviour
{
    public GameObject pt1;
    public GameObject pt2;

    private LineRenderer line;
    // Use this for initialization
    private void Start()
    {
        line = GetComponent<LineRenderer>() ?? this.gameObject.AddComponent<LineRenderer>();     
        line.startWidth = 0.05f;
        line.positionCount = 2;
    }

    private void Update()
    {
        if (pt1 == null || pt2 == null) return;

        line.SetPosition(0, pt1.transform.position);
        line.SetPosition(1, pt2.transform.position);
    }
}
