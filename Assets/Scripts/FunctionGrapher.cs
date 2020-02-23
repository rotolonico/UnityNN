using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChartAndGraph;
using UnityEngine;

public class FunctionGrapher : MonoBehaviour
{
    public static FunctionGrapher Instance;

    public float thickness = 0.1f;

    [SerializeField] private Transform graphDot;

    [SerializeField] private float graphSpacing = 1f;
    [SerializeField] private float graphMin = -50;
    [SerializeField] private float graphMax = 50;

    private MeshFilter meshFilter;
    private MeshFilter backMeshFilter;
    private GraphChartBase graphChartBase;

    private void Awake()
    {
        Instance = this;
        meshFilter = GetComponent<MeshFilter>();
        backMeshFilter = transform.GetChild(0).GetComponent<MeshFilter>();
        //graphChartBase = GameObject.Find("GraphMultiple").GetComponent<GraphChartBase>();

        GraphFunction(x => 0, Color.red);
        GraphFunctionForY(y => 0, Color.green);
    }

    public void GraphFunction(Func<float, float> action, Color color)
    {
        for (var x = graphMin; x < graphMax; x += graphSpacing) SpawnDot(new Vector2(x, action(x)), color);
    }

    public void GraphFunction2(Func<float, float> action, Color color)
    {
        var points = new List<Vector3>();
        for (var x = graphMin; x < graphMax; x += graphSpacing) points.Add(new Vector3(x, action(x)));
        DrawCurve(points.ToArray());
    }

    public void Graph2DFunction(Func<KeyValuePair<float, float>, KeyValuePair<float, float>> action, Color color)
    {
        var points = new List<Vector3>();
        for (var y = graphMin; y < graphMax; y += graphSpacing)
        for (var x = graphMin; x < graphMax; x += graphSpacing)
        {
            var result = action(new KeyValuePair<float, float>(x, y));
            if (!float.IsNaN(result.Key) && !float.IsNaN(result.Value))
                points.Add(new Vector3(result.Key, result.Value));
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
        if (points.Length > 100)
        {
            var newPoints = new List<Vector3>();
            for (int i = 0; i < points.Length; i += points.Length / 100)
            {
                newPoints.Add(points[i]);
            }

            points = newPoints.ToArray();
        }

        var test = new Vector3[]
        {
            new Vector3(0, 0),
            new Vector3(1, 1),
            new Vector3(2, 2),
        };
        //points = test;

        var thickPoints = new Vector3[points.Length * 2];

        for (var i = 0; i < points.Length; i++)
        {
            var heading = i == 0 ? points[i + 1] - points[i] : points[i] - points[i - 1];
            var direction = heading / heading.magnitude;
            var perpendicularDirection = new Vector3(direction.y, -direction.x).normalized;
            thickPoints[i] = points[i] + perpendicularDirection * thickness;
            thickPoints[thickPoints.Length - i - 1] = points[i] + perpendicularDirection * -thickness;
        }

        var vertices2D = new Vector2[thickPoints.Length];
        for (var i = 0; i < thickPoints.Length; i++)
        {
            vertices2D[i] = new Vector2(thickPoints[i].x, thickPoints[i].y);
        }

        var tr = new Triangulator(vertices2D);
        var indices = tr.Triangulate();
        var mesh = new Mesh {vertices = thickPoints, triangles = indices};
        meshFilter.mesh = mesh;
        var backMesh = new Mesh {vertices = thickPoints, triangles = indices.Reverse().ToArray()};
        backMeshFilter.mesh = backMesh;
    }

    public void DrawGraph(List<Vector2> points)
    {
        graphChartBase.DataSource.StartBatch();
        graphChartBase.DataSource.ClearCategory("Player 1");
        for (var i = 0; i < points.Count; i++)
        {
            var point = points[i];
            graphChartBase.DataSource.AddPointToCategory("Player 1", point.x, point.y);
        }

        graphChartBase.DataSource.EndBatch();
    }
}