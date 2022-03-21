using System;
using System.Collections;
using System.Collections.Generic;
using TTP.Toys;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectRetentionCheck : MonoBehaviour
{
    [SerializeField] private float distance = 2f;
    public bool CheckRetention()
    {
        Debug.DrawRay(transform.position, -Vector3.up * distance, Color.red);
        
        if (Physics.Raycast(transform.position, -Vector3.up, out var hit, distance ))
        {
            if (hit.collider.TryGetComponent(out Toy toy))
            {
                return true;
            }
        }
        return false;
    }
}
