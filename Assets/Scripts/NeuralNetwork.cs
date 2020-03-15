public class NeuralNetwork
{
    public readonly int[] Structure;

    public readonly Layer[] Layers;

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