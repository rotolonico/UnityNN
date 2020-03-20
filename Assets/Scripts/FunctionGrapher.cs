using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FunctionGrapher : MonoBehaviour
{
    public static FunctionGrapher Instance;

    public enum DrawMode
    {
        Dot,
        Mesh,
        Line,
        Texture
    }

    [SerializeField] private float thickness = 0.1f;
    [SerializeField] private int pointsLimit = 500;

    [SerializeField] private Transform graphDot;

    [SerializeField] private float graphMinOffset = -2.5f;
    [SerializeField] private float graphMaxOffset = 2.5f;
    [HideInInspector] public int graphDetail = 5;
    [HideInInspector] public float graphSpacingAbs = 0.04f;

    [SerializeField] public DrawMode drawMode = DrawMode.Dot;

    private float graphSpacing;
    private float graphMinX;
    private float graphMaxX;
    private float graphMinY;
    private float graphMaxY;

    [SerializeField] private LineRenderer[] lineRenderer;
    [SerializeField] private Renderer textureRenderer;
    private MeshFilter meshFilter;
    private MeshFilter backMeshFilter;
    private Texture2D texture;

    private readonly List<Chunk> currentChunks = new List<Chunk>();
    private Func<KeyValuePair<float, float>, Point> currentAction;

    private bool detailFunction;
    private float detailFunctionDelay;

    private void Awake()
    {
        Instance = this;

        meshFilter = GetComponent<MeshFilter>();
        backMeshFilter = transform.GetChild(0).GetComponent<MeshFilter>();

        texture = new Texture2D(0, 0);
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

    private void Update()
    {
        detailFunctionDelay += Mathf.Min(Time.deltaTime, 0.09f);
        if (!detailFunction || !(detailFunctionDelay > 0.1f)) return;
        detailFunction = false;
        if (!UIHandler.Instance.customColors.isOn) StartCoroutine(DetailCurrentFunction());
    }

    public void GraphCurrentFunction(Func<KeyValuePair<float, float>, Point> action)
    {
        CalculateMinMax();
        
        currentChunks.Clear();
        currentAction = action;
        for (var y = graphMinY; y < graphMaxY - graphSpacing / 2; y += graphSpacing)
        for (var x = graphMinX; x < graphMaxX - graphSpacing / 2; x += graphSpacing)
        {
            var result = action(new KeyValuePair<float, float>(x, y));
            currentChunks.Add(new Chunk(result));
        }

        DrawCurrentFunction(false);
        detailFunctionDelay = 0;
        detailFunction = true;
    }

    private IEnumerator DetailCurrentFunction()
    {
        yield return null;
        
        var edgeIndexes = GetEdgeChunksIndexes(currentChunks);
        foreach (var chunk in edgeIndexes.Select(edgeIndex => currentChunks[edgeIndex]))
        {
            var mainPoint = chunk.Points[0];
            chunk.Points = new Point[(int) Math.Pow(graphDetail, 2)];
            for (var i = 0; i < chunk.Points.Length; i++) chunk.Points[i] = mainPoint;
            
            for (var y = 0; y < graphDetail; y++)
            for (var x = 0; x < graphDetail; x++)
            {
                var result = currentAction(new KeyValuePair<float, float>(
                    chunk.Points[y * graphDetail + x].X + x * graphSpacing / graphDetail - graphSpacing / graphDetail,
                    chunk.Points[y * graphDetail + x].Y + y * graphSpacing / graphDetail - graphSpacing / graphDetail));
                chunk.Points[y * graphDetail + x] = result;
            }
        }
        
        DrawCurrentFunction(true);
    }

    private void DrawCurrentFunction(bool detailed)
    {
        var drawDetail = detailed ? graphDetail : 1;
        var squaredDrawDetail = (int) Math.Pow(drawDetail, 2);
        var points = new Point[(int) Math.Sqrt(currentChunks.Count) * drawDetail,
            (int) Math.Sqrt(currentChunks.Count) * drawDetail];
        for (var c = 0; c < currentChunks.Count; c++)
        {
            var chunk = currentChunks[c];
            for (var p = 0; p < squaredDrawDetail; p++)
            {
                var pointIndex = Math.Min(p, chunk.Points.Length - 1);
                var x = drawDetail * (int) Mathf.Floor(c / (float) Math.Sqrt(currentChunks.Count)) +
                        (int) Mathf.Floor((float) p / drawDetail);
                var y = drawDetail * (c % (int) Math.Sqrt(currentChunks.Count)) + p % drawDetail;
                var point = chunk.Points[pointIndex];
                points[x, y] = point;
            }
        }

        var pointsList = points.Cast<Point>().ToList();


        switch (drawMode)
        {
            case DrawMode.Dot:
                var dotPoints = GetEdgeChunksIndexes(currentChunks);
                foreach (var point in dotPoints.Select(p => new Vector2(currentChunks[p].Points[0].X, currentChunks[p].Points[0].Y)))
                    DrawDot(point, Color.magenta);
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
                DrawTexture(pointsList.Select(p =>
                {
                    var blackAndWhite = UIHandler.Instance.customColors.isOn;
                    return UIHandler.Instance.dynamicColors.isOn ? p.Color :
                        p.Type == 1 ? blackAndWhite ? Color.white : Color.blue : blackAndWhite ? Color.black : Color.red;
                }).ToArray(), !detailed);
                break;
        }
    }

    private List<int> GetEdgeChunksIndexes(List<Chunk> points)
    {
        var edgeIndexes = new List<int>();
        var pointsPerRow = Mathf.FloorToInt((graphMaxX - graphMinX) / graphSpacing);

        for (var i = 0; i < points.Count; i++)
        {
            //if (i % pointsPerRow == 0 || i == 0 || (i - 1) % pointsPerRow == 0) continue;

            var left = i != 0 && points[i].Points[0].Type != points[i - 1].Points[0].Type;
            var right = i < points.Count - 1 && points[i].Points[0].Type != points[i + 1].Points[0].Type;
            var up = i > pointsPerRow - 1 && points[i].Points[0].Type != points[i - pointsPerRow].Points[0].Type;
            var down = i < points.Count - pointsPerRow &&
                       points[i].Points[0].Type != points[i + pointsPerRow].Points[0].Type;

            if (!left && !right && !up && !down) continue;
            
            if (!edgeIndexes.Contains(i)) edgeIndexes.Add(i);
            if (left && !edgeIndexes.Contains(i - 1)) edgeIndexes.Add(i - 1);
            if (right && !edgeIndexes.Contains(i + 1)) edgeIndexes.Add(i + 1);
            if (up && !edgeIndexes.Contains(i - pointsPerRow)) edgeIndexes.Add(i - pointsPerRow);
            if (down && !edgeIndexes.Contains(i + pointsPerRow)) edgeIndexes.Add(i + pointsPerRow);

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

    public void DrawTexture(Color[] colors, bool blur)
    {
        texture.Resize((int) Math.Sqrt(colors.Length), (int) Math.Sqrt(colors.Length));
        texture.SetPixels(colors);
        texture.Apply();
        texture.filterMode = blur ? FilterMode.Trilinear : FilterMode.Trilinear;

        var cameraSize = CameraController.Instance.thisCam.orthographicSize;

        textureRenderer.material.mainTexture = texture;
        textureRenderer.transform.localScale =
            new Vector3(cameraSize * (graphMaxOffset - graphMinOffset), cameraSize * (graphMaxOffset - graphMinOffset));
    }
}