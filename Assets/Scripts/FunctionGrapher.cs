using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChartAndGraph;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FunctionGrapher : MonoBehaviour
{
    public static FunctionGrapher Instance;

    private enum DrawMode
    {
        Dot,
        Mesh,
        Line,
        Texture
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
    [SerializeField] private SpriteRenderer textureSpriteRenderer;
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

    public void Graph2DFunction(Func<KeyValuePair<float, float>, Point> action, Color color)
    {
        CalculateMinMax();

        var points = new List<Point>();
        for (var y = graphMinY; y < graphMaxY; y += graphSpacing)
        for (var x = graphMinX; x < graphMaxX; x += graphSpacing)
        {
            var result = action(new KeyValuePair<float, float>(x, y));
            points.Add(result);
        }

        switch (drawMode)
        {
            case DrawMode.Dot:
                var dotPoints = GetEdgePoints(points);
                foreach (var point in dotPoints) DrawDot(point, color);
                break;
            case DrawMode.Mesh:
                var meshPoints = GetEdgePoints(points);
                DrawMesh(meshPoints);
                break;
            case DrawMode.Line:
                var linePoints = GetEdgePoints(points);
                DrawLine(lineRenderer[0], linePoints.ToArray());
                break;
            case DrawMode.Texture:
                DrawTexture(points.Select(p => p.Type == 0 ? Color.red : Color.blue).ToArray());
                break;
        }
    }

    private List<Vector3> GetEdgePoints(List<Point> points)
    {
        var edgePoints = new List<Vector3>();
        var pointsPerRow = Mathf.FloorToInt((graphMaxX - graphMinX) / graphSpacing);

        for (var i = 0; i < points.Count; i++)
        {
            if (points[i].Type != 0) continue;
            if (i % pointsPerRow == 0 || i == 0 || (i - 1) % pointsPerRow == 0) continue;

            if (points[i].Type != points[i - 1].Type ||
                i < points.Count - 1 && points[i].Type != points[i + 1].Type ||
                i > pointsPerRow - 1 && points[i].Type != points[i - pointsPerRow].Type ||
                i < points.Count - pointsPerRow && points[i].Type != points[i + pointsPerRow].Type)
                edgePoints.Add(new Vector3(points[i].X, points[i].Y));
        }
        
        return edgePoints;
    }

    private void DrawDot(Vector2 pos, Color color)
    {
        var newDot = Instantiate(graphDot, pos, Quaternion.identity);
        newDot.GetComponent<SpriteRenderer>().color = color;
    }

    private void DrawMesh(List<Vector3> points)
    {
        meshFilter.mesh = new Mesh();

        if (points.Count < 2) return;

        if (points.Count > pointsLimit)
        {
            var newPoints = new List<Vector3>();
            for (int i = 0; i < points.Count; i += points.Count / pointsLimit)
            {
                if (float.IsNaN(points[i].y) || float.IsInfinity(points[i].y)) return;
                newPoints.Add(points[i]);
            }

            points = newPoints;
        }

        var thickPoints = new Vector3[points.Count * 2];

        for (var i = 0; i < points.Count; i++)
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

    public void DrawTexture(Color[] colors)
    {
        var texture = new Texture2D((int) Math.Sqrt(colors.Length), (int) Math.Sqrt(colors.Length));
        texture.SetPixels(colors);
        texture.filterMode = FilterMode.Trilinear;
        texture.Apply();

        var cameraSize = CameraController.Instance.thisCam.orthographicSize;
        var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
        textureSpriteRenderer.sprite = sprite;
        textureSpriteRenderer.transform.localScale = new Vector3(cameraSize / 10, cameraSize / 10);
    }
}