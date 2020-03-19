using System;
using System.Collections.Generic;


[Serializable]
public class NetworkSave
{
    public NeuralNetwork network;
    public KeyValuePair<List<float[]>, List<float[]>> inputsOutputs;
}
