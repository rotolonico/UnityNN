using System;
using System.Collections;
using System.Collections.Generic;
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
        for (var i = 1; i < network.Layers.Length; i++)
        {
            foreach (var node in network.Layers[i].Nodes)
            {
                foreach (var weightNode in node.GetConnectedNodes())
                    InstantiateWeightSlider(node, weightNode);
                InstantiateBiasSlider(node);
            }
        }
    }

    private void InstantiateWeightSlider(Node referenceNode, Node weightNode)
    {
        var newSlider = InstantiateSlider();
        newSlider.GetComponent<Slider>().onValueChanged.AddListener(weight => referenceNode.SetWeight(weightNode, weight));
    }
    
    private void InstantiateBiasSlider(Node referenceNode)
    {
        var newSlider = InstantiateSlider();
        newSlider.GetComponent<Slider>().onValueChanged.AddListener(referenceNode.SetBias);
    }

    private Transform InstantiateSlider()
    {
        var newSlider = Instantiate(slider, transform.position, Quaternion.identity);
        newSlider.SetParent(sliderContainer, false);
        return newSlider;
    }
}