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

    private void Awake() => Instance = this;

    public void DisplayNetwork(NeuralNetwork network)
    {
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

    private void InstantiateWeightSlider(string label, Node referenceNode, Node weightNode)
    {
        var newSlider = InstantiateSlider(label);
        newSlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(weight => referenceNode.SetWeight(weightNode, weight));
    }
    
    private void InstantiateBiasSlider(string label, Node referenceNode)
    {
        var newSlider = InstantiateSlider(label);
        newSlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(referenceNode.SetBias);
    }

    private Transform InstantiateSlider(string label)
    {
        var newSlider = Instantiate(slider, transform.position, Quaternion.identity);
        newSlider.SetParent(sliderContainer, false);
        newSlider.GetComponentInChildren<TextMeshProUGUI>().text = label;
        return newSlider;
    }
}