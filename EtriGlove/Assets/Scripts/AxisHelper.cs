using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisHelper
{
    public enum Axis
    {
        All,
        X, Y, Z,
        M_X, M_Y, M_Z,
        None,
    }

    public static float GetAxis(Vector3 angle, Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                return angle.x;
            case Axis.Y:
                return angle.y;
            case Axis.Z:
                return angle.z;
            case Axis.M_X:
                return -angle.z;
            case Axis.M_Y:
                return -angle.y;
            case Axis.M_Z:
                return -angle.z;
            case Axis.None:
                return 0;
        }

        return angle.x;
    }
}
