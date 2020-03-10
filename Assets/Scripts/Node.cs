using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    private readonly Dictionary<Node, float> weights;
    private float bias;
    public float TrainingBiasSmudge { get; set; }
    public Dictionary<Node, float> TrainingWeightsSmudge { get; set; }

    private bool isInput;
    private float inputValue;

    public float Value { get; private set; }
    public float RawValue { get; private set; }
    private float desiredValue;

    public Node(bool isInput = false)
    {
        weights = new Dictionary<Node, float>();
        TrainingWeightsSmudge = new Dictionary<Node, float>();
        bias = 0;
        this.isInput = isInput;
    }

    public Node(Node[] nodes, bool isInput = false)
    {
        weights = new Dictionary<Node, float>();
        TrainingWeightsSmudge = new Dictionary<Node, float>();
        foreach (var node in nodes)
        {
            weights.Add(node, RandomnessHandler.RandomZeroToOne() * Mathf.Sqrt(2f / nodes.Length));
            TrainingWeightsSmudge.Add(node, 0);
        }
        bias = 0;
        this.isInput = isInput;
    }

    public Node(Dictionary<Node, float> weights, float bias, bool isInput = false)
    {
        this.weights = weights;
        TrainingWeightsSmudge = weights.Clone();
        foreach (var w in TrainingWeightsSmudge) TrainingWeightsSmudge[w.Key] = 0;
        this.bias = bias;
        this.isInput = isInput;
    }

    public float CalculateValue()
    {
        RawValue = isInput ? inputValue : weights.Sum(weight => weight.Key.CalculateValue() * weight.Value) + bias;
        Value = isInput ? inputValue : BasicFunctions.Sigmoid(RawValue);
        desiredValue = Value;
        
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

    public float CalculateCostDelta() => Mathf.Pow(desiredValue - Value, 3);

    public Node[] GetConnectedNodes() => weights.Keys.ToArray();
}