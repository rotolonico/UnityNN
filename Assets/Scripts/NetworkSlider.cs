using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSlider : MonoBehaviour
{
    public enum SliderType
    {
        Weight,
        Bias
    }

    public SliderType type;
    public Node referenceNode;
    public Node weightNode;

    public void Initiate(SliderType newType, Node newReferenceNode, Node newWeightNode)
    {
        type = newType;
        referenceNode = newReferenceNode;
        weightNode = newWeightNode;
    }
}
