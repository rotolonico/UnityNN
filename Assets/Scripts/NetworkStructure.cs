using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkStructure
{
    public int Inputs;
    public int[] HiddenLayers;
    public int Outputs;
    
    public NetworkStructure(int Inputs, int[] HiddenLayers, int Outputs)
    {
        this.Inputs = Inputs;
        this.HiddenLayers = HiddenLayers;
        this.Outputs = Outputs;
    }
}
