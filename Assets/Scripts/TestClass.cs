using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestClass : MonoBehaviour
{
    public float[] inputs;

    public Slider[] slidersW;
    public Slider[] slidersB;

    private void Start()
    {
        var network = new NetworkStructure(2, new[] {3}, 2);
    }

    public void Test()
    {
        FunctionGrapher.Instance.Graph2DFunction(
            i =>
            {
                var a1 = SigmoidFunction(i.Key * slidersW[0].value + i.Value * slidersW[1].value + slidersB[0].value);
                var a2 = SigmoidFunction(i.Key * slidersW[2].value + i.Value * slidersW[3].value + slidersB[1].value);
                var a3 = SigmoidFunction(i.Key * slidersW[4].value + i.Value * slidersW[5].value + slidersB[2].value);
                var rp = SigmoidFunction(a1 * slidersW[6].value + a2 * slidersW[7].value + a3 * slidersW[8].value + slidersB[3].value) -
                         SigmoidFunction(a1 * slidersW[9].value + a2 * slidersW[10].value + a3 * slidersW[11].value + slidersB[4].value);
                return rp > 0
                    ? new KeyValuePair<float, float>(float.NaN, float.NaN)
                    : i;
            }, Color.magenta);
    }

    private float SigmoidFunction(float x) => 1f / (1f + Mathf.Pow(2.718f, -x));
}