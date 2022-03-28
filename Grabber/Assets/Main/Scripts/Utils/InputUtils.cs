using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputUtils
{
    public static bool DetectMoving()
    {
        return    Input.GetKey(KeyCode.A) 
                  || Input.GetKey(KeyCode.W) 
                  || Input.GetKey(KeyCode.D) 
                  || Input.GetKey(KeyCode.S);
    }

    public static bool DetectButtonClick()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}
