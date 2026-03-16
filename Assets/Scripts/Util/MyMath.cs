using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyMath
{
    public static float ReverseClockwiseLerp(float fromAngle, float toAngle, float t)
    {
        fromAngle = Mathf.Repeat(fromAngle, 360);
        toAngle = Mathf.Repeat(toAngle, 360);

        if (toAngle >= fromAngle)
            toAngle -= 360;

        return Mathf.Lerp(fromAngle, toAngle, t);
    }
}
