public class NeuralNetwork
{
    public int[] Structure;

    public Layer[] Layers;

    public NeuralNetwork(int InputsNumber, int[] HiddenLayersNumber, int OutputsNumber)
    {
        Structure = new int[1 + HiddenLayersNumber.Length + 1];

        Structure[0] = InputsNumber;
        for (var i = 1; i < Structure.Length - 1; i++) Structure[i] = HiddenLayersNumber[i - 1];
        Structure[Structure.Length - 1] = OutputsNumber;

        Layers = new Layer[Structure.Length];
        for (var i = 0; i < Layers.Length; i++)
        {
            if (i == 0) Layers[i] = new Layer(Structure[i]);
            else Layers[i] = new Layer(Structure[i], Layers[i - 1]);
        }
    }
}