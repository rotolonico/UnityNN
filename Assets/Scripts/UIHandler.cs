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
    public TMP_InputField saveNameIF;
    public Slider graphDetailSlider;
    public Slider graphSpacingSlider;
    public Slider weightDecaySlider;
    public Slider momentumSlider;
    public Slider classificationOverPrecisionSlider;
    public Toggle dynamicColors;
    public Toggle constantLearning;
    public Toggle blackandWhite;
    public Toggle useANNLibrary;

    private NeuralNetwork network;

    private Perceptron perceptron = new Perceptron();
    private PerceptronLernByBackPropagation PLBBP = new PerceptronLernByBackPropagation();

    private void Start()
    {
        Instance = this;
        if (!useANNLibrary.isOn) network = new NeuralNetwork(new[] {2, 3, 2});
        else perceptron.CreatePerceptron(2, new[] {3}, 2);
        NetworkDisplayer.Instance.DisplayNetwork(network);
        GraphNetwork();
    }

    public void UpdateDetail() => FunctionGrapher.Instance.graphDetail = (int) graphDetailSlider.value;

    public void UpdateSpacing() => FunctionGrapher.Instance.graphSpacingAbs = graphSpacingSlider.value;

    public void UpdateWeightDecay() => NetworkCalculator.weightDecay = weightDecaySlider.value;

    public void UpdateMomentum() => NetworkCalculator.momentum = momentumSlider.value;
    
    public void UpdateClassificationOverPrecision() => NetworkCalculator.classificationOverPrecision = classificationOverPrecisionSlider.value;

    private void Update()
    {
        if (constantLearning.isOn) TrainNetwork();

        if (!Input.GetKey(KeyCode.LeftControl)) return;

        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = GetWorldMousePosition();
            FlowerSpawner.Instance.SpawnRedFlower(mousePosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            var mousePosition = GetWorldMousePosition();
            FlowerSpawner.Instance.SpawnBlueFlower(mousePosition);
        }

        if (Input.GetMouseButtonDown(2))
        {
            var mousePosition = GetWorldMousePosition();

            var inputs = (new[] {mousePosition.x, mousePosition.y});
            var output = TestNetwork(inputs);

            Debug.Log(
                $"INPUTS: {inputs[0]} {inputs[1]} OUTPUTS: {string.Join(" ", output)} {(output[0] >= output[1] ? "Red" : "Blue")}");
            Debug.Log(output[0] >= output[1] ? "Red" : "Blue");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var redFlower in GameObject.FindGameObjectsWithTag("RedFlower")) Destroy(redFlower);
            foreach (var blueFlower in GameObject.FindGameObjectsWithTag("BlueFlower")) Destroy(blueFlower);
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
        var networkStructure = new int[1 + splitStructure.Length + 1];
        networkStructure[0] = 2;
        for (var i = 0; i < splitStructure.Length; i++)
            networkStructure[i + 1] = int.Parse(splitStructure[i]);
        networkStructure[splitStructure.Length + 1] = 2;

        ChangeNetwork(networkStructure);
    }

    private void ChangeNetwork(int[] networkStructure)
    {
        if (!useANNLibrary.isOn) network = new NeuralNetwork(networkStructure);
        else perceptron.CreatePerceptron(2, networkStructure, 2);
        NetworkDisplayer.Instance.DisplayNetwork(network);
        GraphNetwork();
    }

    private float[] TestNetwork(float[] inputs)
    {
        if (!useANNLibrary.isOn)
            return NetworkCalculator.TestNetwork(network, inputs);
        perceptron.Input = inputs;
        perceptron.Solution();
        return perceptron.Output;
    }

    private float[] NormalizeInputs(float[] inputs, float min, float max)
    {
        var normalizedInputs = new float[inputs.Length];
        for (var i = 0; i < inputs.Length; i++) normalizedInputs[i] = (inputs[i] - min) / (max - min);
        return normalizedInputs;
    }

    public void TrainNetwork()
    {
        if (string.IsNullOrEmpty(trainingIterationsIF.text)) return;
        
        var iterations = int.Parse(trainingIterationsIF.text);
        var inputsOutputs = GetInputsOutputs();
        
        for (var i = 0; i < iterations; i++)
        {
            if (!useANNLibrary.isOn)
                NetworkCalculator.TrainNetwork(network, inputsOutputs.Key, inputsOutputs.Value);
            else
                PLBBP.Learn(perceptron, inputsOutputs.Key.ToArray(), inputsOutputs.Value.ToArray());
        }

        NetworkDisplayer.Instance.UpdateSliders();
        GraphNetwork();
    }

    private KeyValuePair<List<float[]>, List<float[]>> GetInputsOutputs()
    {
        var redFlowersPositions = GameObject.FindGameObjectsWithTag("RedFlower").ToList()
            .Select(f => f.transform.position).ToArray();
        var blueFlowersPositions = GameObject.FindGameObjectsWithTag("BlueFlower").ToList()
            .Select(f => f.transform.position).ToArray();

        var inputs = redFlowersPositions.Select(pos => new[] {pos.x, pos.y}).ToList();
        inputs.AddRange(blueFlowersPositions.Select(pos => new[] {pos.x, pos.y}));

        var outputs = redFlowersPositions.Select(pos => new float[] {1, 0}).ToList();
        outputs.AddRange(blueFlowersPositions.Select(pos => new float[] {0, 1}));

        return new KeyValuePair<List<float[]>, List<float[]>>(inputs, outputs);
    }

    public void GraphNetwork()
    {
        if (FunctionGrapher.Instance.drawMode == FunctionGrapher.DrawMode.Dot)
            foreach (var dot in GameObject.FindGameObjectsWithTag("Dot"))
                Destroy(dot);

        FunctionGrapher.Instance.Graph2DFunction(
            i => GetPointFromOutput(i.Key, i.Value, TestNetwork(new[] {i.Key, i.Value})), Color.magenta);
    }

    public void SaveNetwork()
    {
        if (string.IsNullOrEmpty(saveNameIF.text)) return;

        var networkSave = new NetworkSave
        {
            structure = network.Structure,
            inputsOutputs = GetInputsOutputs(),
            decay = weightDecaySlider.value,
            momentum = momentumSlider.value,
            classificationOverPrecision = classificationOverPrecisionSlider.value
        };

        NetworkStorage.SaveNetwork(networkSave, saveNameIF.text);
    }

    public void LoadNetwork()
    {
        if (string.IsNullOrEmpty(saveNameIF.text)) return;

        var networkSave = NetworkStorage.LoadNetwork(saveNameIF.text);
        if (networkSave == null) return;

        ChangeNetwork(networkSave.structure);

        weightDecaySlider.value = networkSave.decay;
        momentumSlider.value = networkSave.momentum;
        classificationOverPrecisionSlider.value = networkSave.classificationOverPrecision; 

        var inputs = networkSave.inputsOutputs.Key;
        var outputs = networkSave.inputsOutputs.Value;
        
        foreach (var redFlower in GameObject.FindGameObjectsWithTag("RedFlower")) Destroy(redFlower);
        foreach (var blueFlower in GameObject.FindGameObjectsWithTag("BlueFlower")) Destroy(blueFlower);

        for (var i = 0; i < inputs.Count; i++)
        {
            if (Math.Abs(outputs[i][0]) < 0.01f)
                FlowerSpawner.Instance.SpawnBlueFlower(new Vector2(inputs[i][0], inputs[i][1]));
            else
                FlowerSpawner.Instance.SpawnRedFlower(new Vector2(inputs[i][0], inputs[i][1]));
        }
    }

    private Point GetPointFromOutput(float x, float y, IReadOnlyList<float> output)
    {
        int type;
        var color = new Color();

        if (output[0] > output[1])
        {
            type = 0;
            if (!blackandWhite.isOn) color = new Color(1 - (output[0] - output[1]) * 0.75f, 0, 0);
        }
        else if (output[0] < output[1])
        {
            type = 1;
            if (!blackandWhite.isOn) color = new Color(0, 0, 1 - (output[1] - output[0]) * 0.75f);
        }
        else
        {
            type = 2;
            if (!blackandWhite.isOn) color = Color.white;
        }
        
        if (blackandWhite.isOn)
            color = new Color((1 + output[1] - output[0]) / 2, (1 + output[1] - output[0]) / 2,
                (1 + output[1] - output[0]) / 2);

        return new Point(x, y, type, color);
    }
}