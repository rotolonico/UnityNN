using UnityEngine;

public static class ActivationFunctions
{
    public static string[] Names = { "Old Sigmoid", "Identity", "Binary step", "ISRLU", "ReLU", "Logistic", "TanH", "Sinusoid", "Sinc", "Gaussian" };

    /// <summary>
    /// Activation function of the neuron.
    /// </summary>
    /// <param name="Sum">The sum of all values of neurons multiplied with weight.</param>
    /// <param name="AFT">Type of activation function.</param>
    /// <param name="AFS">Activation function scale. 1 - default scale.</param>
    /// <param name="AFWM">Activation function with minus. If "true", then all neurons perceive values from -1 to 1. This also applies to output neurons. If "false" - from 0 to 1.</param>
    /// <returns>Returns float value.</returns>
    public static float ActivationFunction(float Sum, int AFT, float AFS, bool AFWM)
    {
        Sum = Sum * AFS; //Identity function (AFT == 1)

        float Minus = 0;
        if (AFWM)
            Minus = -1;

        if (AFT == 0)           //Old Sigmoid function
        {
            Sum = (1F - Minus) / (1F + Mathf.Exp(-(Sum - (1 + Minus) / 2F))) + Minus;
        }
        else if (AFT == 2)      //Binary step function
        {
            if (Sum < 0)
                Sum = Minus;
            else
                Sum = 1;
        }
        else if (AFT == 3)      //ISRLU function
        {
            Sum = Sum / AFS;
            if (Sum < 0)
                Sum = Sum / Mathf.Sqrt(1 + AFS * Sum * Sum);
            Sum = Sum / (2 + Minus) + (1 + Minus) / 2F;
        }
        else if (AFT == 4)      //ReLU function
        {
            if (Sum < 0)
                Sum = 0;
        }
        else if (AFT == 5)      //Logistic function
        {
            Sum = (1F - Minus) / (1F + Mathf.Exp(-Sum)) + Minus;
        }
        else if (AFT == 6)      //TanH function
        {
            float E = Mathf.Exp(Sum);
            float mE = Mathf.Exp(-Sum);
            Sum = (E - mE) / (E + mE) / (2 + Minus) + (1 + Minus) / 2F;
        }
        else if (AFT == 7)      //Sinusoid function
        {
            Sum = Mathf.Sin(Sum) / (2 + Minus) + (1 + Minus) / 2F; ;
        }
        else if (AFT == 8)      //Sinc function
        {
            if (Sum == 0)
                Sum = 1;
            else
                Sum = Mathf.Sin(Sum) / Sum;
            Sum = Sum / (2 + Minus) + (1 + Minus) / 2F;
        }
        else if (AFT == 9)      //Gaussian function
        {
            Sum = Mathf.Exp(-Sum * Sum) * (1 - Minus) + Minus;
        }

        if (Sum < Minus || Sum > 1)
            Sum = Mathf.Clamp(Sum, Minus, 1);

        return Sum;
    }

    /// <summary>
    /// Search for a derivative of the value of a particular neuron.
    /// </summary>
    /// <param name="X">The value of a neuron.</param>
    /// <param name="AFT">Activation function scale.</param>
    /// <param name="AFT">Activation function scale.</param>
    /// <param name="AFWM">Activation function with minus.</param>
    /// <returns></returns>
    public static float Derivative(float X, int AFT, float AFS, bool AFWM)
    {
        int Minus = 0;
        if (AFWM)
            Minus = -1;
        if (AFT == 0)           //Old Sigmoid function
            X = ((X - Minus) * (1F - X)) / (1F - Minus);
        else if (AFT == 1)      //Identity function
        {
            X = 1;
        }
        else if (AFT == 2)      //Binary step function
        {
            if (X != Minus)
                X = Minus;
            else
                X = Random.Range(0, 1F);
        }
        else if (AFT == 3)      //ISRLU function
        {
            X = (X - (1 + Minus) / 2F) * (2 + Minus);
            if (X < 0)
                X = Mathf.Pow(1 / Mathf.Sqrt(1 + AFS * X * X), 3) / AFS;
            else
                X = 1;
        }
        else if (AFT == 4)      //ReLU function
        {
            if (X < 0)
                X = 0;
            else
                X = 1;
        }
        else if (AFT == 5)      //Logistic function
        {
            X = (X - Minus) * (1 - X) / (1 - Minus);
        }
        else if (AFT == 6)      //TanH function
        {
            X = (X - (1 + Minus) / 2F) * (2 + Minus);
            X = 1 - X * X;
        }
        else if (AFT == 7)      //Sinusoid function
        {
            X = (X - (1 + Minus) / 2F) * (2 + Minus);
            X = Mathf.Cos(X);
        }
        else if (AFT == 8)      //Sinc function
        {
            X = (X - (1 + Minus) / 2F) * (2 + Minus);
            if (X == 0)
                X = 0;
            else
                X = (Mathf.Cos(X) / X - Mathf.Sin(X) / (X * X));
        }
        else if (AFT == 9)      //Gaussian function
        {
            X = X / (1 - Minus) - Minus;
            X = -2 * X * Mathf.Exp(-X * X);
        }
        return X * AFS;
    }

    public static Texture ActivationFunctionView(int AFT, float AFS, bool AFWM)
    {
        Texture2D View = new Texture2D(71, 25, TextureFormat.ARGB32, false);
        Color[] CA = new Color[1775];

        for (int i = 0; i < CA.Length; i++)
        {
            CA[i] = Color.clear;
        }
        View.SetPixels(CA);
        int x = 0;
        while (x < 25)
        {
            View.SetPixel(35, x, new Color(1, 1, 1, 0.5F));
            View.SetPixel(29, x, new Color(1, 1, 1, 0.25F));
            View.SetPixel(41, x, new Color(1, 1, 1, 0.25F));
            x++;
        }
        x = 0;
        while (x < 71)
        {
            if (AFWM)
                View.SetPixel(x, 12, new Color(1, 1, 1, 0.5F));
            else
                View.SetPixel(x, 0, new Color(1, 1, 1, 0.5F));
            x++;
        }
        float xx = 0;
        while (xx < 70)
        {
            int M = 0;
            if (AFWM)
                M = 1;
            float AF = ActivationFunction((12 * AFS / 70F) * (xx - 35), AFT, AFS, AFWM);
            int y = Mathf.RoundToInt((Derivative(AF, AFT, AFS, AFWM) + M) * 12.5F * (2 - M));
            y = Mathf.Clamp(y, 0, 24);
            x = Mathf.RoundToInt(xx);
            View.SetPixel(x, y, new Color(0, 1, 0));

            y = Mathf.RoundToInt((AF + M) * 12.5F * (2 - M));
            y = Mathf.Clamp(y, 0, 24);
            View.SetPixel(x, y, new Color(1, 0, 0));

            xx += 0.25F;
        }
        View.Apply();
        return View;
    }
}