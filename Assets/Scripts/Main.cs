using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance;

    public TMP_InputField networkStructureIF;

    private NeuralNetwork network;

    private void Start()
    {
        Instance = this;
        network = new NeuralNetwork(2, new[] {3}, 2);
        NetworkDisplayer.Instance.DisplayNetwork(network);
        GraphNetwork();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = -1;
            mousePosition = CameraController.Instance.thisCam.ScreenToWorldPoint(mousePosition);
            var output = NetworkCalculator.TestNetwork(network, new[] {mousePosition.x, mousePosition.y});
            Debug.Log(string.Join(" ", output));
            Debug.Log(output[0] >= output[1] ? "Red" : "Blue");
        }
    }

    public void ChangeNetwork()
    {
        var splitStructure = networkStructureIF.text != "" ? networkStructureIF.text.Split('.') : new string[0];
        var networkStructure = new int[splitStructure.Length];
        for (var i = 0; i < splitStructure.Length; i++)
            networkStructure[i] = int.Parse(splitStructure[i]);

        network = new NeuralNetwork(2, networkStructure, 2);
        NetworkDisplayer.Instance.DisplayNetwork(network);
        GraphNetwork();
    }

    public void GraphNetwork()
    {
        foreach (var dot in GameObject.FindGameObjectsWithTag("Dot")) Destroy(dot);

        FunctionGrapher.Instance.GraphFunction(x => 0, Color.red);
        FunctionGrapher.Instance.GraphFunctionForY(y => 0, Color.green);

        FunctionGrapher.Instance.Graph2DFunction(
            i =>
            {
                var output = NetworkCalculator.TestNetwork(network, new[] {i.Key, i.Value});
                return Math.Abs(output[0] - output[1]) > 0.01f
                    ? new KeyValuePair<float, float>(float.NaN, float.NaN)
                    : i;
            }, Color.magenta);
    }
}