using System.Collections;
using UnityEngine;

/// <summary>
/// The node of the Artificial Neural Network.
/// Non-MonoBehaviour script.
/// </summary>
public class ANNNode
{
    /// <summary>
    /// The value of neuron.
    /// </summary>
    public float Neuron = 0;

    /// <summary>
    /// The value of bias of neuron.
    /// </summary>
    public float Bias = 0;

    /// <summary>
    /// List of input connections numbers.
    /// </summary>
    public ArrayList ConnectionIn;

    /// <summary>
    /// The fullness of the neuron connections when solving.
    /// </summary>
    public int Fullness;

    /// <summary>
    /// Position at visualization.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// The number of the neuron activation function. If AFT == -1 then AFT = default activation function of ANN.
    /// </summary>
    public int AFT = -1;

    /// <summary>
    /// Neuron memory usage.
    /// </summary>
    public bool UseMemory = false;

    /// <summary>
    /// The value of neuron memory.
    /// </summary>
    public float NeuronMemory = 0;

    /// <summary>
    /// The value of neuron memory weight.
    /// </summary>
    public float WeightMemory = 0;

    /// <summary>
    /// The node of the Artificial Neural Network.
    /// </summary>
    /// <param name="Position">Position at visualization.</param>
    public ANNNode(Vector2 Position)
    {
        Neuron = 0;
        this.Position = Position;
        Fullness = 0;
        ConnectionIn = new ArrayList();
        Bias = 0;
    }

    /// <summary>
    /// The node of the Artificial Neural Network.
    /// </summary>
    /// <param name="Bias">The value of bias of neuron.</param>
    /// <param name="Position">Position at visualization.</param>
    /// <param name="AFT">The number of the neuron activation function.</param>
    public ANNNode(float Bias, Vector2 Position, int AFT, bool UseMemory, float WeightMemory)
    {
        Neuron = 0;
        this.Position = Position;
        Fullness = 0;
        ConnectionIn = new ArrayList();
        this.Bias = Bias;
        this.AFT = AFT;
        this.UseMemory = UseMemory;
        this.WeightMemory = WeightMemory;
    }

    /// <summary>
    /// The node of the Artificial Neural Network.
    /// </summary>
    /// <param name="Position">Position at visualization.</param>
    /// <param name="Fullness">The fullness of the neuron connections when solving.</param>
    /// <param name="AFT">The number of the neuron activation function.</param>
    public ANNNode(Vector2 Position, int Fullness, int AFT)
    {
        Neuron = 0;
        this.Position = Position;
        this.Fullness = Fullness;
        this.AFT = AFT;
        ConnectionIn = new ArrayList();
        Bias = 0;
    }

    /// <summary>
    /// The node of the Artificial Neural Network.
    /// </summary>
    /// <param name="Bias">The value of bias of neuron.</param>
    /// <param name="Position">Position at visualization.</param>
    /// <param name="Fullness">The fullness of the neuron connections when solving.</param>
    /// <param name="AFT">The number of the neuron activation function.</param>
    public ANNNode(float Bias, Vector2 Position, int Fullness, int AFT)
    {
        Neuron = 0;
        this.Position = Position;
        this.Fullness = Fullness;
        ConnectionIn = new ArrayList();
        this.Bias = Bias;
        this.AFT = AFT;
    }

    /// <summary>
    /// Node info is translated into a string.
    /// </summary>
    public override string ToString()
    {
        string Weights = "";
        int w = 0;
        while (w < ConnectionIn.Count)
        {
            Weights += ";";
            Weights += ConnectionIn[w];
            w++;
        }
        if (UseMemory)
            return (Bias + ";" + Position.x + ";" + Position.y + Weights + "|" + AFT + ";" + UseMemory + ";" + WeightMemory);
        else
            return (Bias + ";" + Position.x + ";" + Position.y + Weights + "|" + AFT);
    }

    /// <summary>
    /// Load node info from the string.
    /// </summary>
    /// <param name="Info">Node string info.</param>
    public ANNNode(string Info)
    {
        if (ConnectionIn != null)
            ConnectionIn.Clear();
        else
            ConnectionIn = new ArrayList();
        string I = "";
        int c = 0;
        int stage = 0;
        while (c < Info.Length)
        {
            if (Info[c] != ';' && Info[c] != '|' && c != Info.Length - 1)
                I += Info[c];
            else
            {
                if (c == Info.Length - 1)
                    I += Info[c];
                if (stage == 0)
                    Bias = Formulas.StringToFloat(I);
                else if (stage == 1)
                    Position.x = Formulas.StringToFloat(I);
                else if (stage == 2)
                    Position.y = Formulas.StringToFloat(I);
                else if (stage == 3)
                {
                    ConnectionIn.Add(Formulas.StringToInt(I));
                    stage--;
                    if (Info[c] == '|')
                        stage++;
                }
                else if (stage == 4)
                    AFT = Formulas.StringToInt(I);
                else if (stage == 5)
                    UseMemory = Formulas.StringToBool(I);
                else if (stage == 6)
                    WeightMemory = Formulas.StringToFloat(I);
                I = "";
                stage++;
            }
            c++;
        }
    }
}