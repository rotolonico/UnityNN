using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NetworkCalculator
{
    public static float weightDecay = 0.001f;
    
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

    public static void TrainNetwork(NeuralNetwork network, List<float[]> inputs, List<float[]> desiredOutputs)
    {
        foreach (var desiredOutput in desiredOutputs.Where(desiredOutput => network.Structure[network.Structure.Length - 1] != desiredOutput.Length))
        {
            Debug.Log("Expected " + network.Structure[network.Structure.Length - 1] + " outputs, got " +
                      desiredOutput.Length);
            return;
        }
        
        for (var i = 0; i < inputs.Count; i++)
        {
            var input = inputs[i];
            TestNetwork(network, input);

            for (var j = 0; j < network.Layers[network.Layers.Length - 1].Nodes.Length; j++)
                network.Layers[network.Layers.Length - 1].Nodes[j].SetDesiredValue(desiredOutputs[i][j]);

            for (var j = network.Layers.Length - 1; j >= 1; j--)
            {
                foreach (var node in network.Layers[j].Nodes)
                {
                    var biasSmudge = BasicFunctions.SigmoidDerivative(node.Value) * node.CalculateCostDelta();
                    node.TrainingBiasSmudge += biasSmudge;

                    foreach (var connectedNode in node.GetConnectedNodes())
                    {
                        var weightSmudge = connectedNode.Value * biasSmudge;
                        node.TrainingWeightsSmudge[connectedNode] += weightSmudge;
                        
                        var valueSmudge = node.GetWeight(connectedNode) * biasSmudge;
                        connectedNode.SmudgeDesiredValue(valueSmudge);
                    }
                }
            }
        }

        for (var i = network.Layers.Length - 1; i >= 1; i--)
        {
            foreach (var node in network.Layers[i].Nodes)
            {
                node.SmudgeBias(node.TrainingBiasSmudge);
                node.SetBias(node.GetBias() * (1 - weightDecay));
                node.TrainingBiasSmudge *= 0.1f;

                foreach (var connectedNode in node.GetConnectedNodes())
                {
                    node.SmudgeWeight(connectedNode, node.TrainingWeightsSmudge[connectedNode]);
                    node.TrainingWeightsSmudge[connectedNode] *= 0.1f;
                    
                    node.SetWeight(connectedNode, node.GetWeight(connectedNode) * (1 - weightDecay));
                }
                
                node.SetDesiredValue(0);
            }
        }

    }
}