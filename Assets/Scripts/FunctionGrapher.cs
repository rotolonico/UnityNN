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
    private int graphDetail = 1;

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
    private Texture2D texture;

    public Slider graphDetailSlider;

    private void Awake()
    {
        Instance = this;

        meshFilter = GetComponent<MeshFilter>();
        backMeshFilter = transform.GetChild(0).GetComponent<MeshFilter>();

        texture = new Texture2D(0, 0)
        {
            filterMode = FilterMode.Trilinear
        };
    }

    public void UpdateDetail() => graphDetail = (int) graphDetailSlider.value;

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

        var test = new List<Point>();
        var chunks = new List<Chunk>();
        for (var y = graphMinY; y < graphMaxY - graphSpacing / 2; y += graphSpacing)
        for (var x = graphMinX; x < graphMaxX - graphSpacing / 2; x += graphSpacing)
        {
            var result = action(new KeyValuePair<float, float>(x, y));
            chunks.Add(new Chunk(result, graphDetail));
            test.Add(result);
        }

        var edgeIndexes = GetEdgeChunksIndexes(chunks);
        foreach (var edgeIndex in edgeIndexes)
        {
            var chunk = chunks[edgeIndex];
            for (var y = 0; y < chunk.Size; y++)
            for (var x = 0; x < chunk.Size; x++)
            {
                var result = action(new KeyValuePair<float, float>(
                    chunk.Points[y * graphDetail + x].X + x * graphSpacing / graphDetail - graphSpacing / graphDetail,
                    chunk.Points[y * graphDetail + x].Y + y * graphSpacing / graphDetail - graphSpacing / graphDetail));
                chunk.Points[y * graphDetail + x] = result;
            }
        }

        var points = new Point[(int) Math.Sqrt(chunks.Count) * graphDetail, (int) Math.Sqrt(chunks.Count) * graphDetail];
        for (var c = 0; c < chunks.Count; c++)
        {
            var chunk = chunks[c];
            for (var p = 0; p < chunk.Points.Length; p++)
            {
                var x = chunk.Size * (int) Mathf.Floor(c / (float) Math.Sqrt(chunks.Count)) +
                        (int) Mathf.Floor((float) p / chunk.Size);
                var y = chunk.Size * (c % (int) Math.Sqrt(chunks.Count)) + p % chunk.Size;
                var point = chunk.Points[p];
                points[x,y] = point;
            }
        }

        var pointsList = points.Cast<Point>().ToList();


        switch (drawMode)
        {
            case DrawMode.Dot:
                var dotPoints = GetEdgeChunksIndexes(chunks);
                foreach (var point in dotPoints.Select(p => new Vector2(chunks[p].Points[0].X, chunks[p].Points[0].Y))) DrawDot(point, color);
                break;
            /*case DrawMode.Mesh:
                var meshPoints = GetEdgeChunksIndexes(chunks);
                DrawMesh(meshPoints);
                break;
            case DrawMode.Line:
                var linePoints = GetEdgeChunksIndexes(chunks);
                DrawLine(lineRenderer[0], linePoints.ToArray());
                break;*/
            case DrawMode.Texture:
                DrawTexture(pointsList.Select(p => p.Color).ToArray());
                break;
        }
    }

    private List<int> GetEdgeChunksIndexes(List<Chunk> points)
    {
        var edgeIndexes = new List<int>();
        var pointsPerRow = Mathf.FloorToInt((graphMaxX - graphMinX) / graphSpacing);

        for (var i = 0; i < points.Count; i++)
        {
            if (i % pointsPerRow == 0 || i == 0 || (i - 1) % pointsPerRow == 0) continue;

            if (points[i].Points[0].Type != points[i - 1].Points[0].Type ||
                i < points.Count - 1 && points[i].Points[0].Type != points[i + 1].Points[0].Type ||
                i > pointsPerRow - 1 && points[i].Points[0].Type != points[i - pointsPerRow].Points[0].Type ||
                i < points.Count - pointsPerRow && points[i].Points[0].Type != points[i + pointsPerRow].Points[0].Type)
                edgeIndexes.Add(i);
        }

        return edgeIndexes;
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
        texture.Resize((int) Math.Sqrt(colors.Length), (int) Math.Sqrt(colors.Length));
        texture.SetPixels(colors);
        texture.Apply();

        var cameraSize = CameraController.Instance.thisCam.orthographicSize;

        textureSpriteRenderer.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f), 1);
        textureSpriteRenderer.transform.localScale =
            new Vector3(cameraSize * graphSpacingAbs / graphDetail, cameraSize * graphSpacingAbs / graphDetail);
    }
}