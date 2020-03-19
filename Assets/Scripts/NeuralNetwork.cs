public class NeuralNetwork
{
    public readonly int[] Structure;
    public readonly Layer[] Layers;
    
    public float WeightDecay = 0.001f;
    public float ClassificationOverPrecision = 1;
    public float Momentum = 0;
    public float MaxError = 0;

    public bool Done;

    public NeuralNetwork(int[] structure)
    {
        Structure = structure;
        
        Layers = new Layer[Structure.Length];
        for (var i = 0; i < Layers.Length; i++)
        {
            if (i == 0) Layers[i] = new Layer(Structure[i]);
            else Layers[i] = new Layer(Structure[i], Layers[i - 1]);
        }
    }
}