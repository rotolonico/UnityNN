
public static class RandomnessHandler
{
    private static System.Random Random = new System.Random();
    
    public static float RandomMinusOneToOne() => RandomMinMax(-1, 1);

    public static float RandomMinMax(float min, float max) => (float) Random.NextDouble() * (max - min) + min;
}
