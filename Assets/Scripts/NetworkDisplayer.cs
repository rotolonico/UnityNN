using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkDisplayer : MonoBehaviour
{
    public static NetworkDisplayer Instance;
    
    public Transform sliderContainer;
    public Transform slider;

    private readonly List<NetworkSlider> currentSliders = new List<NetworkSlider>();

    private void Awake() => Instance = this;

    public void DisplayNetwork(NeuralNetwork network)
    {
        currentSliders.Clear();
        
        var weightsCount = 0;
        var biasCount = 0;
        
        foreach (var sliderToDestroy in GameObject.FindGameObjectsWithTag("Slider")) Destroy(sliderToDestroy);

        for (var i = 1; i < network.Layers.Length; i++)
        {
            foreach (var node in network.Layers[i].Nodes)
            {
                foreach (var weightNode in node.GetConnectedNodes())
                {
                    weightsCount++;
                    InstantiateWeightSlider($"w{weightsCount}", node, weightNode);
                }
                biasCount++;
                InstantiateBiasSlider($"b{biasCount}", node);
            }
        }
    }

    public void UpdateSliders()
    {
        foreach (var currentSlider in currentSliders)
        {
            var currentSliderComponent = currentSlider.GetComponentInChildren<Slider>();
            
            switch (currentSlider.type)
            {
                case NetworkSlider.SliderType.Weight:
                    currentSliderComponent.SetValueWithoutNotify(currentSlider.referenceNode.GetWeight(currentSlider.weightNode));
                    break;
                case NetworkSlider.SliderType.Bias:
                    currentSliderComponent.SetValueWithoutNotify(currentSlider.referenceNode.GetBias());
                    break;
            }
        }
    }

    private void InstantiateWeightSlider(string label, Node referenceNode, Node weightNode)
    {
        var newSlider = InstantiateSlider(label);
        newSlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(weight =>
        {
            referenceNode.SetWeight(weightNode, weight);
            UIHandler.Instance.GraphNetwork();
        });
        
        var newNetworkComponent = newSlider.GetComponent<NetworkSlider>();
        newNetworkComponent.Initiate(NetworkSlider.SliderType.Weight, referenceNode, weightNode);
        currentSliders.Add(newNetworkComponent);
    }
    
    private void InstantiateBiasSlider(string label, Node referenceNode)
    {
        var newSlider = InstantiateSlider(label);
        newSlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(f =>
        {
            referenceNode.SetBias(f);
            UIHandler.Instance.GraphNetwork();
        });
        
        var newNetworkComponent = newSlider.GetComponent<NetworkSlider>();
        newNetworkComponent.Initiate(NetworkSlider.SliderType.Bias, referenceNode, null);
        currentSliders.Add(newNetworkComponent);
    }

    private Transform InstantiateSlider(string label)
    {
        var newSlider = Instantiate(slider, transform.position, Quaternion.identity);
        newSlider.SetParent(sliderContainer, false);
        newSlider.GetComponentInChildren<TextMeshProUGUI>().text = label;
        return newSlider;
    }
}