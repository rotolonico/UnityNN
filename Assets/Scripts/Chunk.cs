using System;

public class Chunk
{
    public int Size { get; }
    public Point[] Points { get; set; }

    public Chunk(Point[] points)
    {
        Size = (int) Math.Sqrt(points.Length);
        Points = points;
    }

    public Chunk(Point point, int size)
    {
        Size = size;
        Points = new Point[(int) Math.Pow(size, 2)];
        for (var i = 0; i < Math.Pow(size, 2); i++) Points[i] = point;
    }

}
