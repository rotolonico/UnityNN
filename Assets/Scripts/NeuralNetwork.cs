public class NeuralNetwork
{
    public readonly int[] Structure;

    public readonly Layer[] Layers;

    public NeuralNetwork(int inputsNumber, int[] hiddenLayersNumber, int outputsNumber)
    {
        Structure = new int[1 + hiddenLayersNumber.Length + 1];

        Structure[0] = inputsNumber;
        for (var i = 1; i < Structure.Length - 1; i++) Structure[i] = hiddenLayersNumber[i - 1];
        Structure[Structure.Length - 1] = outputsNumber;
        Layers = new Layer[Structure.Length];
        for (var i = 0; i < Layers.Length; i++)
        {
            if (i == 0) Layers[i] = new Layer(Structure[i]);
            else Layers[i] = new Layer(Structure[i], Layers[i - 1]);
        }
    }
}