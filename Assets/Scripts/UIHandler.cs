using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance;

    public bool allowMovement;
    public bool allowPlacing;

    public Slider networkStructureSlider;
    public Slider trainingIterations;
    public TMP_InputField saveNameIF;
    public Slider graphDetailSlider;
    public Slider graphSpacingSlider;
    public Slider weightDecaySlider;
    public Slider momentumSlider;
    public Slider classificationOverPrecisionSlider;
    public Slider maxErrorSlider;
    public Toggle dynamicColors;
    public Toggle constantLearning;
    public Toggle customColors;
    public GameObject stuckButton;

    public Color customColor1;
    public Color customColor2;

    private NeuralNetwork network;

    private void Start()
    {
        Instance = this;
        CameraController.Instance.isActive = allowMovement;
        network = new NeuralNetwork(new[] {2, 3, 2});
        NetworkDisplayer.Instance.DisplayNetwork(network);
        GraphNetwork();
    }

    public void UpdateDetail() => FunctionGrapher.Instance.graphDetail = (int) graphDetailSlider.value;

    public void UpdateSpacing() => FunctionGrapher.Instance.graphSpacingAbs = graphSpacingSlider.value;

    public void UpdateWeightDecay() => network.WeightDecay = weightDecaySlider.value;

    public void UpdateMomentum() => network.Momentum = momentumSlider.value;

    public void UpdateClassificationOverPrecision() =>
        network.ClassificationOverPrecision = classificationOverPrecisionSlider.value;

    public void UpdateMaxError()
    {
        network.MaxError = maxErrorSlider.value;
        network.Done = false;
    }

    private void Update()
    {
        if (constantLearning.isOn) TrainNetwork();

        if (!allowPlacing || InputUIBlocker.BlockedByUI) return;

        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = GetWorldMousePosition();
            FlowerSpawner.Instance.SpawnRedFlower(mousePosition);
            network.Done = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            var mousePosition = GetWorldMousePosition();
            FlowerSpawner.Instance.SpawnBlueFlower(mousePosition);
            network.Done = false;
        }
    }

    public void ClearFlowers()
    {
        foreach (var redFlower in GameObject.FindGameObjectsWithTag("RedFlower")) Destroy(redFlower);
        foreach (var blueFlower in GameObject.FindGameObjectsWithTag("BlueFlower")) Destroy(blueFlower);
    }

    private Vector3 GetWorldMousePosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = -1;
        return CameraController.Instance.thisCam.ScreenToWorldPoint(mousePosition);
    }

    public void ChangeNetwork()
    {
        var networkStructure = new int[3];
        networkStructure[0] = 2;
        networkStructure[1] = (int) networkStructureSlider.value;
        networkStructure[2] = 2;

        ChangeNetwork(networkStructure);
    }

    private void ChangeNetwork(int[] networkStructure)
    {
        network = new NeuralNetwork(networkStructure);
        NetworkDisplayer.Instance.DisplayNetwork(network);
        GraphNetwork();
    }

    private float[] TestNetwork(float[] inputs) => NetworkCalculator.TestNetwork(network, inputs);

    private float[] NormalizeInputs(float[] inputs, float min, float max)
    {
        var normalizedInputs = new float[inputs.Length];
        for (var i = 0; i < inputs.Length; i++) normalizedInputs[i] = (inputs[i] - min) / (max - min);
        return normalizedInputs;
    }

    public void TrainNetwork()
    {
        var iterations = trainingIterations.value;
        var inputsOutputs = GetInputsOutputs();

        for (var i = 0; i < iterations; i++)
        {
            if (network.Done)
            {
                NetworkDisplayer.Instance.UpdateSliders();
                GraphNetwork();
                return;
            }

            NetworkCalculator.TrainNetwork(network, inputsOutputs.Key, inputsOutputs.Value);
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

        FunctionGrapher.Instance.GraphCurrentFunction(
            i => GetPointFromOutput(i.Key, i.Value, TestNetwork(new[] {i.Key, i.Value})));
    }

    public void SaveNetwork()
    {
        if (string.IsNullOrEmpty(saveNameIF.text)) return;

        for (var i = network.Layers.Length - 1; i >= 1; i--)
            foreach (var node in network.Layers[i].Nodes)
                node.PrepareNodeToSave();

        var networkSave = new NetworkSave
        {
            network = network,
            inputsOutputs = GetInputsOutputs(),
        };

        NetworkStorage.SaveNetwork(networkSave, saveNameIF.text);
    }

    public void LoadNetwork()
    {
        if (string.IsNullOrEmpty(saveNameIF.text)) return;

        var networkSave = NetworkStorage.LoadNetwork(saveNameIF.text);
        if (networkSave == null) return;
        ImplementNetwork(networkSave);
    }

    public void LoadNetworkFromResources(string networkName)
    {
        var networkSaveJson = Resources.Load<TextAsset>($"{networkName}").text;
        var networkSave = NetworkStorage.LoadNetworkFromJSON(networkSaveJson);
        ImplementNetwork(networkSave);
    }

    private void ImplementNetwork(NetworkSave networkSave)
    {
        network = networkSave.network;

        weightDecaySlider.value = networkSave.network.WeightDecay;
        momentumSlider.value = networkSave.network.Momentum;
        classificationOverPrecisionSlider.value = networkSave.network.ClassificationOverPrecision;

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

        NetworkDisplayer.Instance.DisplayNetwork(network);
        GraphNetwork();
    }

    private Point GetPointFromOutput(float x, float y, IReadOnlyList<float> output)
    {
        int type;
        var color = new Color();

        if (output[0] > output[1])
        {
            type = 0;
            if (!customColors.isOn) color = new Color(1 - (output[0] - output[1]) * 0.75f, 0, 0);
        }
        else if (output[0] < output[1])
        {
            type = 1;
            if (!customColors.isOn) color = new Color(0, 0, 1 - (output[1] - output[0]) * 0.75f);
        }
        else
        {
            type = 2;
            if (!customColors.isOn) color = Color.white;
        }

        if (customColors.isOn)
            color = Color.Lerp(customColor1, customColor2, (1 + output[1] - output[0]) / 2);

        return new Point(x, y, type, color);
    }

    public void GetHint()
    {
        StartCoroutine(GetHintCoroutine());
    }

    private IEnumerator GetHintCoroutine()
    {
        stuckButton.SetActive(false);
        
        for (var i = 0; i < 50; i++)
        {
            yield return null;
            NetworkCalculator.LerpToCorrectParams(network, 0.01f);
            NetworkDisplayer.Instance.UpdateSliders();
            GraphNetwork();
        }

        yield return new WaitForSecondsRealtime(30);
        stuckButton.SetActive(true);
    }

    public void GoToDemo1() => SceneManager.LoadScene(0);

    public void GoToDemo2() => SceneManager.LoadScene(1);
}