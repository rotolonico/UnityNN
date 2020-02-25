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

    public static void TrainNetwork(NeuralNetwork network, float[] inputs, float[] desiredOutputs)
    {
        if (network.Structure[network.Structure.Length - 1] != desiredOutputs.Length)
        {
            Debug.Log("Expected " + network.Structure[network.Structure.Length - 1] + " outputs, got " + desiredOutputs.Length);
            return;
        }

        var outputs = TestNetwork(network, inputs);
        if (outputs == null) return;
        
        for (var i = 0; i < network.Layers[network.Layers.Length - 1].Nodes.Length; i++)
            network.Layers[network.Layers.Length - 1].Nodes[i].SetDesiredValue(desiredOutputs[i]);

        for (var i = network.Layers.Length - 1; i >= 1; i--)
        {
            foreach (var node in network.Layers[i].Nodes)
            {
                var biasSmudge = BasicFunctions.SigmoidDerivative(node.Value) * 2 * node.CalculateCostDelta();
                node.SmudgeBias(biasSmudge);

                foreach (var connectedNode in node.GetConnectedNodes())
                {
                    var weightSmudge = connectedNode.Value * biasSmudge;
                    var valueSmudge = node.GetWeight(connectedNode) * biasSmudge;
                    
                    node.SmudgeWeight(connectedNode, weightSmudge);
                    connectedNode.SmudgeDesiredValue(valueSmudge);
                }
            }
        }
    }
}