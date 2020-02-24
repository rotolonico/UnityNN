using System.Collections.Generic;
using System.Linq;

public class Node
{
    private Dictionary<Node, float> weights;
    private float bias;

    private bool isInput;
    private float inputValue;

    public Node(bool isInput = false)
    {
        weights = new Dictionary<Node, float>();
        bias = RandomnessHandler.RandomMinusOneToOne();
        this.isInput = isInput;
    }

    public Node(Node[] nodes, bool isInput = false)
    {
        weights = new Dictionary<Node, float>();
        foreach (var node in nodes) weights.Add(node, RandomnessHandler.RandomMinusOneToOne());
        bias = RandomnessHandler.RandomMinusOneToOne();
        this.isInput = isInput;
    }

    public Node(Dictionary<Node, float> weights, float bias, bool isInput = false)
    {
        this.weights = weights;
        this.bias = bias;
        this.isInput = isInput;
    }

    public float CalculateValue()
    {
        if (isInput) return inputValue;
        return BasicFunctions.Sigmoid(weights.Sum(weight => weight.Key.CalculateValue() * weight.Value) + bias);
    }
    
    public void SetInputValue(float newInputValue) => inputValue = newInputValue;
    
    public void SetWeight(Node node, float weight) => weights[node] = weight;
    
    public void SetBias(float newBias) => bias = newBias;

    public Node[] GetConnectedNodes() => weights.Keys.ToArray();
}