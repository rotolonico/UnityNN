using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    private Dictionary<Node, float> weights;
    private float bias;

    private bool isInput;
    private float inputValue;

    public float Value { get; private set; }
    private float desiredValue;

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
        Value = isInput ? inputValue : BasicFunctions.Sigmoid(weights.Sum(weight => weight.Key.CalculateValue() * weight.Value) + bias);
        return Value;
    }
    
    public void SetInputValue(float newInputValue) => inputValue = newInputValue;
    
    public float GetWeight(Node node) => weights[node];
    
    public void SetWeight(Node node, float weight) => weights[node] = weight;
    
    public void SmudgeWeight(Node node, float weight) => weights[node] += weight;
    
    public float GetBias() => bias;
    
    public void SetBias(float newBias) => bias = newBias;
    public void SmudgeBias(float newBias) => bias += newBias;

    public void SetDesiredValue(float newDesiredValue) => desiredValue = newDesiredValue;
    
    public void SmudgeDesiredValue(float newDesiredValue) => desiredValue += newDesiredValue;

    public float CalculateCostDelta() => desiredValue - Value;

    public Node[] GetConnectedNodes() => weights.Keys.ToArray();
}