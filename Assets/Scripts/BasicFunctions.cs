using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFunctions : MonoBehaviour
{
    public static float Sigmoid(float x) => 1f / (1f + Mathf.Pow(2.718f, -x));

    public static float HardSigmoid(float x)
    {
        if (x < -2.5f)
            return 0;
        if (x > 2.5f)
            return 1;
        return 0.2f * x + 0.5f;
    }
}
