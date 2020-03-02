
public static class RandomnessHandler
{
    private static readonly System.Random Random = new System.Random();
    
    public static float RandomMinusOneToOne() => RandomMinMax(-1, 1);

    public static float RandomZeroToOne() => (float) Random.NextDouble();

    public static float RandomMinMax(float min, float max) => (float) Random.NextDouble() * (max - min) + min;
    
    public static void Randomize<T>(T[] items)
    {
        for (int i = 0; i < items.Length - 1; i++)
        {
            int j = Random.Next(i, items.Length);
            T temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }
    }
}
