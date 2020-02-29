using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance;

    public TMP_InputField networkStructureIF;
    public TMP_InputField trainingIterationsIF;
    public Slider precisionSlider;

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
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            var mousePosition = GetWorldMousePosition();
            FlowerSpawner.Instance.SpawnRedFlower(mousePosition);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1))
        {
            var mousePosition = GetWorldMousePosition();
            FlowerSpawner.Instance.SpawnBlueFlower(mousePosition);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(2))
        {
            var mousePosition = GetWorldMousePosition();
            var output = NetworkCalculator.TestNetwork(network, new[] {mousePosition.x, mousePosition.y});
            Debug.Log(string.Join(" ", output));
            Debug.Log(output[0] >= output[1] ? "Red" : "Blue");
        }
    }

    private Vector3 GetWorldMousePosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = -1;
        return CameraController.Instance.thisCam.ScreenToWorldPoint(mousePosition);
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

    public void TrainNetwork()
    {
        var redFlowersPositions = GameObject.FindGameObjectsWithTag("RedFlower").ToList()
            .Select(f => f.transform.position).ToArray();
        var blueFlowersPositions = GameObject.FindGameObjectsWithTag("BlueFlower").ToList()
            .Select(f => f.transform.position).ToArray();

        var inputs = redFlowersPositions.Select(pos => new[] {pos.x, pos.y}).ToList();
        inputs.AddRange(blueFlowersPositions.Select(pos => new[] {pos.x, pos.y}));

        var outputs = redFlowersPositions.Select(pos => new float[] {1, 0}).ToList();
        outputs.AddRange(blueFlowersPositions.Select(pos => new float[] {0, 1}));

        var iterations = int.Parse(trainingIterationsIF.text);
        for (var i = 0; i < iterations; i++)
        for (var j = 0; j < inputs.Count; j++)
            NetworkCalculator.TrainNetwork(network, inputs[j], outputs[j]);

        NetworkDisplayer.Instance.UpdateSliders();
        GraphNetwork();
    }

    public void GraphNetwork()
    {
        foreach (var dot in GameObject.FindGameObjectsWithTag("Dot")) Destroy(dot);

        FunctionGrapher.Instance.Graph2DFunction(
            i =>
            {
                var output = NetworkCalculator.TestNetwork(network, new[] {i.Key, i.Value});
                return new Point(i.Key, i.Value, output[0] > output[1] ? 0 : 1);
            }, Color.magenta);
    }
}