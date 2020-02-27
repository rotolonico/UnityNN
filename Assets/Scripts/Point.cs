using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public float X { get; }
    public float Y { get; }
    public int Type { get; }

    public Point(float x, float y, int type)
    {
        X = x;
        Y = y;
        Type = type;
    }

}
