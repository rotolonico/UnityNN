using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkCalculator
{
    public static float[] TestNetwork(NeuralNetwork network, float[] inputs)
    {
        if (network.Structure[0] != inputs.Length)
        {
            Debug.Log("Expected " + network.Structure[0] + " inputs, got " + inputs.Length);
            return null;
        }

        for (var i = 0; i < network.Layers[0].Nodes.Length; i++) network.Layers[0].Nodes[i].SetInputValue(inputs[i]);

        var outputs = new float[network.Structure[network.Structure.Length - 1]];

        for (var i = 0; i < network.Layers[network.Layers.Length - 1].Nodes.Length; i++)
            outputs[i] = network.Layers[network.Layers.Length - 1].Nodes[i].CalculateValue();

        return outputs;
    }
}