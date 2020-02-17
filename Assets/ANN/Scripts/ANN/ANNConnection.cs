/// <summary>
/// Connection between neurons.
/// Non-MonoBehaviour script.
/// </summary>
public class ANNConnection
{
    /// <summary>
    /// The number of input neuron.
    /// </summary>
    public int In;

    /// <summary>
    /// The number of output neuron.
    /// </summary>
    public int Out;

    /// <summary>
    /// The weight of the connection.
    /// </summary>
    public float Weight;

    /// <summary>
    /// Connection activity.
    /// </summary>
    public bool Enable = true;

    /// <summary>
    /// If "true" - the connection gets the data of the neuron from the previous solution.
    /// </summary>
    public bool IsMemory = false;

    /// <summary>
    /// The value of the input neuron of the weight from the previous solution.
    /// </summary>
    public float PreviousData;

    /// <summary>
    /// Connection between neurons.
    /// </summary>
    /// <param name="In">The number of input neuron.</param>
    /// <param name="Out">The number of output neuron.</param>
    /// <param name="Weight">The weight of the connection.</param>
    /// <param name="Enable">Connection activity.</param>
    /// <param name="IsMemory">The сonnection gets the data of the neuron from the previous solution.</param>
    public ANNConnection(int In, int Out, float Weight, bool Enable, bool IsMemory)
    {
        this.In = In;
        this.Out = Out;
        this.Weight = Weight;
        this.Enable = Enable;
        this.IsMemory = IsMemory;
    }

    /// <summary>
    /// Connection between neurons. IsMemory = false.
    /// </summary>
    /// <param name="In">The number of input neuron.</param>
    /// <param name="Out">The number of output neuron.</param>
    /// <param name="Weight">The weight of the connection.</param>
    /// <param name="Enable">Connection activity.</param>
    public ANNConnection(int In, int Out, float Weight, bool Enable)
    {
        this.In = In;
        this.Out = Out;
        this.Weight = Weight;
        this.Enable = Enable;
        IsMemory = false;
    }

    /// <summary>
    /// Connection between neurons. Connection activity (Enable) = true. IsMemory = false.
    /// </summary>
    /// <param name="In">The number of input neuron.</param>
    /// <param name="Out">The number of output neuron.</param>
    /// <param name="Weight">The weight of the connection.</param>
    public ANNConnection(int In, int Out, float Weight)
    {
        this.In = In;
        this.Out = Out;
        this.Weight = Weight;
        Enable = true;
        IsMemory = false;
    }

    /// <summary>
    /// Connection between neurons. Connection activity (Enable) = true.
    /// </summary>
    /// <param name="In">The number of input neuron.</param>
    /// <param name="Out">The number of output neuron.</param>
    /// <param name="IsMemory">The сonnection gets the data of the neuron from the previous solution.</param>
    public ANNConnection(int In, int Out, bool IsMemory)
    {
        this.In = In;
        this.Out = Out;
        Weight = 0;
        Enable = true;
        this.IsMemory = IsMemory;
    }


    /// <summary>
    /// Connection between neurons. Connection weight (Weight) = 0. Connection activity (Enable) = true. IsMemory = false.
    /// </summary>
    /// <param name="In">The number of input neuron.</param>
    /// <param name="Out">The number of output neuron.</param>
    public ANNConnection(int In, int Out)
    {
        this.In = In;
        this.Out = Out;
        Weight = 0;
        Enable = true;
        IsMemory = false;
    }

    /// <summary>
    /// Connection info is translated into a string.
    /// </summary>
    public override string ToString()
    {
        return (In + ";" + Out + ";" + Weight + ";" + Enable + ";" + IsMemory);
    }

    /// <summary>
    /// Load connection info from the string.
    /// </summary>
    /// <param name="Info">Connection string info.</param>
    public ANNConnection(string Info)
    {
        string I = "";
        int c = 0;
        int stage = 0;
        while (c < Info.Length)
        {
            if (Info[c] != ';' && c != Info.Length - 1)
                I += Info[c];
            else
            {
                if (c == Info.Length - 1)
                    I += Info[c];
                if (stage == 0)
                    In = Formulas.StringToInt(I);
                else if (stage == 1)
                    Out = Formulas.StringToInt(I);
                else if (stage == 2)
                    Weight = Formulas.StringToFloat(I);
                else if (stage == 3)
                    Enable = Formulas.StringToBool(I);
                else
                    IsMemory = Formulas.StringToBool(I);
                I = "";
                stage++;
            }
            c++;
        }
    }
}