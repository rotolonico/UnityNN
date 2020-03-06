using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public float X { get; }
    public float Y { get; }
    public int Type { get; }
    public Color Color { get; }

    public Point(float x, float y, int type, Color color)
    {
        X = x;
        Y = y;
        Type = type;
        Color = color;
    }

}
