using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChartAndGraph;
using UnityEngine;

public class FunctionGrapher : MonoBehaviour
{
    public static FunctionGrapher Instance;

    private enum DrawMode
    {
        Dot,
        Mesh,
        Line
    }

    [SerializeField] private float thickness = 0.1f;
    [SerializeField] private int pointsLimit = 500;

    [SerializeField] private Transform graphDot;

    [SerializeField] private float graphSpacingAbs = 0.2f;
    [SerializeField] private float graphMinOffset = -5;
    [SerializeField] private float graphMaxOffset = 5;

    [SerializeField] private DrawMode drawMode = DrawMode.Dot;

    private float graphSpacing;
    private float graphMinX;
    private float graphMaxX;
    private float graphMinY;
    private float graphMaxY;

    [SerializeField] private LineRenderer[] lineRenderer;
    private MeshFilter meshFilter;
    private MeshFilter backMeshFilter;
    private GraphChartBase graphChartBase;

    private void Awake()
    {
        Instance = this;
        
        meshFilter = GetComponent<MeshFilter>();
        backMeshFilter = transform.GetChild(0).GetComponent<MeshFilter>();
    }

    private void CalculateMinMax()
    {
        var mainCamera = CameraController.Instance.thisCam;
        var cameraPos = mainCamera.transform.position;
        var cameraSize = mainCamera.orthographicSize;
        graphMinX = cameraPos.x + graphMinOffset * cameraSize;
        graphMaxX = cameraPos.x + graphMaxOffset * cameraSize;
        graphMinY = cameraPos.y + graphMinOffset * cameraSize;
        graphMaxY = cameraPos.y + graphMaxOffset * cameraSize;
        graphSpacing = cameraSize * graphSpacingAbs;
    }

    public void GraphFunction(Func<float, float> action, Color color)
    {
        CalculateMinMax();
        
        var points = new List<Vector3>();
        for (var x = graphMinX; x < graphMaxX; x += graphSpacing) points.Add(new Vector3(x, action(x)));
        DrawLine(lineRenderer[1], points.ToArray());
    }

    public void GraphFunctionForY(Func<float, float> action, Color color)
    {
        CalculateMinMax();
        
        var points = new List<Vector3>();
        for (var y = graphMinY; y < graphMaxY; y += graphSpacing) points.Add(new Vector3(action(y), y));
        DrawLine(lineRenderer[2], points.ToArray());
    }

    public void Graph2DFunction(Func<KeyValuePair<float, float>, KeyValuePair<float, float>> action, Color color)
    {
        CalculateMinMax();

        var points = new List<Vector3>();
        for (var y = graphMinY; y < graphMaxY; y += graphSpacing)
        for (var x = graphMinX; x < graphMaxX; x += graphSpacing)
        {
            var result = action(new KeyValuePair<float, float>(x, y));
            if (float.IsNaN(result.Key) || float.IsNaN(result.Value)) continue;
            switch (drawMode)
            {
                case DrawMode.Dot:
                    DrawDot(new Vector2(result.Key, result.Value), color);
                    break;
                case DrawMode.Mesh:
                case DrawMode.Line:
                    points.Add(new Vector3(result.Key, result.Value));
                    break;
            }
        }

        switch (drawMode)
        {
            case DrawMode.Mesh:
                DrawMesh(points.ToArray());
                break;
            case DrawMode.Line:
                DrawLine(lineRenderer[0], points.ToArray());
                break;
        }
    }

    private void DrawDot(Vector2 pos, Color color)
    {
        var newDot = Instantiate(graphDot, pos, Quaternion.identity);
        newDot.GetComponent<SpriteRenderer>().color = color;
    }

    private void DrawMesh(Vector3[] points)
    {
        meshFilter.mesh = new Mesh();

        if (points.Length < 2) return;

        if (points.Length > pointsLimit)
        {
            var newPoints = new List<Vector3>();
            for (int i = 0; i < points.Length; i += points.Length / pointsLimit)
            {
                if (float.IsNaN(points[i].y) || float.IsInfinity(points[i].y)) return;
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

    public void DrawLine(LineRenderer lr, Vector3[] points)
    {
        lr.positionCount = points.Length;
        lr.SetPositions(points);
    }
}