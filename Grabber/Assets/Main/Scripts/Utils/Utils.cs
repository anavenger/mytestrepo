using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static bool DetectMoving()
    {
        return    Input.GetKey(KeyCode.A) 
                  || Input.GetKey(KeyCode.W) 
                  || Input.GetKey(KeyCode.D) 
                  || Input.GetKey(KeyCode.S);
    }
}
