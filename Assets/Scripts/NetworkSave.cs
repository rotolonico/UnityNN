using System;
using System.Collections.Generic;


[Serializable]
public class NetworkSave
{
    public int[] structure;
    public KeyValuePair<List<float[]>, List<float[]>> inputsOutputs;
    public float decay;
    public float momentum;
    public float classificationOverPrecision;
}
