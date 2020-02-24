using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestClass : MonoBehaviour
{
    public static TestClass Instance;
    
    public float[] inputs;

    public float test;

    public Slider[] slidersW;
    public Slider[] slidersB;

    private NeuralNetwork network;

    private void Start()
    {
        Instance = this;
        network = new NeuralNetwork(2, new[] {3}, 2);
        NetworkDisplayer.Instance.DisplayNetwork(network);
    }

    public void Test()
    {
        var output = NetworkCalculator.TestNetwork(network, inputs);
        Debug.Log(string.Join(" ", output));
        
        foreach (var VARIABLE in GameObject.FindGameObjectsWithTag("Dots"))
        {
            Destroy(VARIABLE);
        }
        
//        FunctionGrapher.Instance.GraphFunction2(
//            x => { return slidersW[0].value * Mathf.Pow(x, 2) + slidersW[1].value * x + slidersB[0].value; }, Color.magenta);

//        FunctionGrapher.Instance.GraphFunction2(
//            x =>
//            {
//                // x2 = (x1w3 + b2 - x1w1 - b1) / (w2 - w4)
//                return (x * slidersW[2].value + slidersB[1].value - x * slidersW[0].value - slidersB[0].value) / (slidersW[1].value - slidersW[3].value);
//            }, Color.magenta);



/*        FunctionGrapher.Instance.Graph2DFunction(
            i =>
            {
                var a1 = SigmoidFunction(
                    i.Key * slidersW[0].value + i.Value * slidersW[1].value + slidersB[0].value);
                var a2 = SigmoidFunction(
                    i.Key * slidersW[2].value + i.Value * slidersW[3].value + slidersB[1].value);
                var a3 = SigmoidFunction(
                    i.Key * slidersW[4].value + i.Value * slidersW[5].value + slidersB[2].value);
                var rp = a1 * slidersW[6].value + a2 * slidersW[7].value + a3 * slidersW[8].value +
                         slidersB[3].value -
                         (a1 * slidersW[9].value + a2 * slidersW[10].value + a3 * slidersW[11].value +
                             slidersB[4].value);
                //Debug.Log(rp);
                return Math.Abs(rp) > FunctionGrapher.Instance.graphSpacing ? new KeyValuePair<float, float>(float.NaN, float.NaN) : i;
            }, Color.magenta);*/
        
        FunctionGrapher.Instance.GraphFunction(x => 0, Color.red);
        FunctionGrapher.Instance.GraphFunctionForY(y => 0, Color.green);
    }
}