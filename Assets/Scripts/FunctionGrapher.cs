using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionGrapher : MonoBehaviour
{
    public static FunctionGrapher Instance;

    [SerializeField] private Transform graphDot;

    [SerializeField] private float graphSpacing = 1f;
    [SerializeField] private float graphMin = -50;
    [SerializeField] private float graphMax = 50;

    private LineRenderer lr;

    private void Awake()
    {
        Instance = this;
        lr = GetComponent<LineRenderer>();
        
        GraphFunction(x => 0, Color.red);
        GraphFunctionForY(y => 0, Color.green);
    }
    
    public void GraphFunction(Func<float, float> action, Color color)
    {
        for (var x = graphMin; x < graphMax; x += graphSpacing) SpawnDot(new Vector2(x, action(x)), color);
    }

    public void Graph2DFunction(Func<KeyValuePair<float, float>, KeyValuePair<float, float>> action, Color color)
    {
        var points = new List<Vector3>();
        for (var y = graphMin; y < graphMax; y += graphSpacing)
        for (var x = graphMin; x < graphMax; x += graphSpacing)
        {
            var result = action(new KeyValuePair<float, float>(x, y));
            if (!float.IsNaN(result.Key) && !float.IsNaN(result.Value)) points.Add(new Vector3(result.Key, result.Value, 0));
        }
        
        DrawCurve(points.ToArray());
    }

    public void GraphFunctionForY(Func<float, float> action, Color color)
    {
        for (var y = graphMin; y < graphMax; y += graphSpacing) SpawnDot(new Vector2(action(y), y), color);
    }

    private void SpawnDot(Vector2 pos, Color color)
    {
        var newDot = Instantiate(graphDot, pos, Quaternion.identity);
        newDot.GetComponent<SpriteRenderer>().color = color;
    }

    private void DrawCurve(Vector3[] points)
    {
        lr.positionCount = points.Length;
        lr.SetPositions(points);
    }
}