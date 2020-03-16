using System;

public class Chunk
{
    public Point[] Points { get; set; }

    public Chunk(Point point)
    {
        Points = new[] {point};
    }

}
