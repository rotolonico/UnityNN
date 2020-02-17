using UnityEngine;
using System.IO;

/// <summary>
/// Interface of NeuroEvolution of Augmenting Topologies method.
/// </summary>
public class ANNLearnByNEATInterface : MonoBehaviour
{
    /// <summary>
    /// Artificial neural network.
    /// </summary>
    public ANN Ann;

    /// <summary>
    /// NEAT learning method.
    /// </summary>
    public ANNLearnByNEAT NL = new ANNLearnByNEAT();

    /// <summary>
    /// Is ANN learning?
    /// </summary>
    public bool Learn = false;

    private Rect LearningWindowRect = new Rect(Screen.width, Screen.height, 580, 305);
    private bool LearningWindowSize = true;

    private bool ShowAdditionalSettings = false;

    private Vector2 NeuronSettingsScroll = new Vector2(0, 0);

    private string SettingsName = "";

    private void Start()
    {
        if (gameObject.name == "StudentChild")
            Destroy(gameObject.GetComponent<ANNLearnByNEATInterface>());
        else
        {
            if (Ann == null)
                Debug.LogWarning("NEAT learn interface does not have ANN.");
            else
                NL.Ann = Ann;
        }
    }

    private void Update()
    {
        if (Learn && Ann != null)
            NL.Learn();
    }

    private void OnGUI()
    {
        if (Ann != null)
        {
            LearningWindowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), LearningWindowRect, WindowInterface, "NEAT in " + transform.name);  // interface window
        }
    }

    private void WindowInterface(int ID)                                                            // interface window
    {
        if (LearningWindowRect.height == 65)                                                        // small window
        {
            if (GUI.Button(new Rect(LearningWindowRect.width - 20, 0, 20, 20), "▄"))                // change window scale
            {
                if (LearningWindowSize)
                {
                    LearningWindowRect.width = 580;
                    LearningWindowRect.height = 305;
                    LearningWindowRect.x -= 380;
                }
                else
                {
                    LearningWindowRect.width = 200;
                    LearningWindowRect.height = 215;
                }
            }
            if (NL.MutationAddNeuron == 0 && NL.MutationAddWeight == 0 && NL.MutationChangeOneWeight == 0 && NL.MutationChangeWeights == 0)
                GUI.enabled = false;
            bool Activate = false;
            Learn = InterfaceGUI.Button(1, 1, "Learn OFF", "Learn ON", Learn, ref Activate);

            if (Activate && !Learn)
                NL.StopLearn();

            GUI.enabled = true;
        }
        else                                                                                        // big window
        {
            if (GUI.Button(new Rect(LearningWindowRect.width - 20, 0, 20, 20), "-"))                // change window scale
            {
                LearningWindowRect.width = 200;
                LearningWindowRect.height = 65;
                if (LearningWindowSize)
                    LearningWindowRect.x += 380;
            }

            if (GUI.Button(new Rect(LearningWindowRect.width - 40, 0, 20, 20), "S"))                // LearningWindowSize on/off
            {
                if (LearningWindowSize)
                {
                    LearningWindowRect.width = 200;
                    LearningWindowRect.height = 215;
                    LearningWindowRect.x += 380;
                    LearningWindowSize = false;
                }
                else
                {
                    LearningWindowRect.width = 580;
                    LearningWindowRect.height = 305;
                    LearningWindowRect.x -= 380;
                    LearningWindowSize = true;
                }
            }

            if (LearningWindowSize)
            {
                if (ShowAdditionalSettings && GUI.Button(new Rect(LearningWindowRect.width - 60, 0, 20, 20), "A"))
                    ShowAdditionalSettings = false;
                if (!ShowAdditionalSettings && GUI.Button(new Rect(LearningWindowRect.width - 60, 0, 20, 20), "M"))
                    ShowAdditionalSettings = true;
                NL.AmountOfChildren = InterfaceGUI.IntArrows(2, 1, "Children amount", NL.AmountOfChildren, 1);

                NL.Cross = InterfaceGUI.Button(2, 2, "Crossing OFF", "Crossing ON", NL.Cross);

                if (Ann.Connection.Count == 0)
                    NL.PerceptronStart = InterfaceGUI.Button(2, 3, "No weights", "Perceptron", NL.PerceptronStart);

                NL.MutationSum = NL.MutationAddNeuron + NL.MutationAddWeight + NL.MutationChangeOneWeight + NL.MutationChangeWeights + NL.MutationChangeOneBias + NL.MutationChangeBias;

                if (NL.MutationSum == 0)
                    NL.MutationSum = 1;

                NL.MutationAddWeight = InterfaceGUI.HorizontalSlider(2, 4, "Ratio add weight", NL.MutationAddWeight, NL.MutationAddWeight / NL.MutationSum * 100F, 0F, 1F);

                NL.MutationChangeOneWeight = InterfaceGUI.HorizontalSlider(2, 5, "Ratio change 1 weight", NL.MutationChangeOneWeight, NL.MutationChangeOneWeight / NL.MutationSum * 100F, 0F, 1F);
                NL.MutationChangeWeights = InterfaceGUI.HorizontalSlider(2, 6, "Ratio change weights", NL.MutationChangeWeights, NL.MutationChangeWeights / NL.MutationSum * 100F, 0F, 1F);
                NL.MutationAddNeuron = InterfaceGUI.HorizontalSlider(2, 7, "Ratio add neuron", NL.MutationAddNeuron, NL.MutationAddNeuron / NL.MutationSum * 100F, 0F, 1F);
                NL.MutationChangeOneBias = InterfaceGUI.HorizontalSlider(2, 8, "Ratio change 1 bias", NL.MutationChangeOneBias, NL.MutationChangeOneBias / NL.MutationSum * 100F, 0F, 1F);
                NL.MutationChangeBias = InterfaceGUI.HorizontalSlider(2, 9, "Ratio change bias", NL.MutationChangeBias, NL.MutationChangeBias / NL.MutationSum * 100F, 0F, 1F);

                NL.ChanceCoefficient = InterfaceGUI.HorizontalSlider(1, 6, "Chance coefficient", NL.ChanceCoefficient, 0F, 0.5F);

                if (!ShowAdditionalSettings)
                {
                    if (Learn)
                        GUI.enabled = false;
                    if (NL.AmountOfChildren > 1)
                    {
                        if (NL.AmountOfChildren != NL.ChildrenInWave)
                            NL.ChildrenByWave = InterfaceGUI.Button(3, 1, "Maximum in one time", "By waves", NL.ChildrenByWave);
                        NL.ChildrenInWave = InterfaceGUI.IntArrows(3, 2, "Children in wave", NL.ChildrenInWave, 1, NL.AmountOfChildren);
                    }
                    NL.IgnoreCollision = InterfaceGUI.Button(3, 3, "Collision ON", "Collision OFF", NL.IgnoreCollision);
                    GUI.enabled = true;
                    if (NL.MutationAddWeight != 0)
                        NL.AddingWeightsCount = InterfaceGUI.HorizontalSlider(3, 4, "Maximum count of weights", NL.AddingWeightsCount, 1, 4);
                    if (NL.MutationChangeOneWeight != 0 || NL.MutationChangeWeights != 0 || NL.MutationChangeOneBias != 0 || NL.MutationChangeBias != 0)
                        NL.ChangeWeightSign = InterfaceGUI.HorizontalSlider(3, 5, "Chance change sign", NL.ChangeWeightSign, NL.ChangeWeightSign * 100, 0F, 1F);
                    if (NL.PerceptronStart || NL.MutationAddWeight != 0 || NL.MutationChangeOneWeight != 0 || NL.MutationChangeWeights != 0 || NL.MutationChangeOneBias != 0 || NL.MutationChangeBias != 0)
                        NL.ChildrenDifference = InterfaceGUI.HorizontalSlider(3, 6, "Children difference", NL.ChildrenDifference, 0.01F, 10F);

                    NL.Autosave = InterfaceGUI.Button(3, 7, "Autosave OFF", "Autosave ON", NL.Autosave);
                    if (NL.Autosave)
                    {
                        NL.AutosaveStep = InterfaceGUI.IntArrows(3, 8, "Autosave step", NL.AutosaveStep, 1);
                        NL.AutosaveName = GUI.TextField(new Rect(390, 265, 180, 30), NL.AutosaveName);
                    }
                }
                else
                {
                    if (NL.MutationAddWeight > 0)
                    {
                        NL.UseMemoryWeight[0] = InterfaceGUI.Button(3, 1, "Hidden X Hidden", "Hidden ↔ Hidden", NL.UseMemoryWeight[0]);
                        NL.UseMemoryWeight[1] = InterfaceGUI.Button(3, 2, "Hidden → Outputs", "Hidden ↔ Outputs", NL.UseMemoryWeight[1]);
                    }
                    if (NL.MutationAddNeuron > 0 || NL.MutationAddWeight > 0)
                        NL.UseNeuronsMemory[0] = InterfaceGUI.HorizontalSlider(3, 3, "M-N in HL chance", NL.UseNeuronsMemory[0], NL.UseNeuronsMemory[0] * 100F, 0F, 1F);
                    if (NL.MutationAddWeight > 0)
                        NL.UseNeuronsMemory[1] = InterfaceGUI.HorizontalSlider(3, 4, "M-N in OL chance", NL.UseNeuronsMemory[1], NL.UseNeuronsMemory[1] * 100F, 0F, 1F);

                    if (NL.MutationAddNeuron > 0)
                    {
                        NeuronSettingsScroll = GUI.BeginScrollView(new Rect(380, 140, 205, 150), NeuronSettingsScroll, new Rect(0, 20, 1, 30 * ActivationFunctions.Names.Length + 5));
                        int i = 0;
                        while (i < ActivationFunctions.Names.Length)
                        {
                            if (NL.AFT.Contains(i))
                            {
                                if (InterfaceGUI.Button(1, i + 1, "[V] - " + i + ". " + ActivationFunctions.Names[i]))
                                    NL.AFT.RemoveAt(NL.AFT.IndexOf(i));
                            }
                            else
                            {
                                if (InterfaceGUI.Button(1, i + 1, "[X] - " + i + ". " + ActivationFunctions.Names[i]))
                                    NL.AFT.Add(i);
                            }
                            i++;
                        }
                        GUI.EndScrollView();
                    }

                }

                if (NL.ChanceCoefficient != 0)
                    InterfaceGUI.Info(1, 7, "Chance", NL.Chance);

                SettingsName = GUI.TextField(new Rect(10, 235, 180, 30), SettingsName);
                if (InterfaceGUI.MiddleButton(1, 9, "Save"))
                    Save();
                if (InterfaceGUI.MiddleButton(2, 9, "Load"))
                    Load();
            }
            else
            {
                if (NL.ChanceCoefficient != 0)
                    InterfaceGUI.Info(1, 6, "Chance", NL.Chance);
            }
            InterfaceGUI.Info(1, 2, "Best generation", NL.BestGeneration);
            InterfaceGUI.Info(1, 3, "Generation", NL.Generation);
            InterfaceGUI.Info(1, 4, "Children", NL.ChildrenInGeneration);
            InterfaceGUI.Info(1, 5, "Best longevity", NL.BestLongevity);


            if (NL.MutationAddNeuron == 0 && NL.MutationAddWeight == 0 && NL.MutationChangeOneWeight == 0 && NL.MutationChangeWeights == 0 && NL.MutationChangeOneBias == 0 && NL.MutationChangeBias == 0)
                GUI.enabled = false;

            bool Activate = false;
            Learn = InterfaceGUI.Button(1, 1, "Learn OFF", "Learn ON", Learn, ref Activate);

            if (Activate && !Learn)
                NL.StopLearn();

            GUI.enabled = true;
        }

        LearningWindowRect = InterfaceGUI.WindowControl(LearningWindowRect);
    }

    private void Save()
    {
        if (SettingsName != "")
        {
            if (!Directory.Exists(Application.dataPath + "/ANN/Settings"))
            {
                Directory.CreateDirectory(Application.dataPath + "/ANN/Settings");
            }
            if (File.Exists(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbn"))
                File.Delete(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbn");

            if (!File.Exists(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbn"))
            {
                StreamWriter SC = File.CreateText(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbn");
                SC.WriteLine("ANN NEAT settings:");
                SC.WriteLine(NL.ChanceCoefficient + ";" + NL.AmountOfChildren + ";" + NL.Cross + ";" + NL.PerceptronStart + ";" + NL.MutationAddWeight + ";"
                     + NL.MutationChangeOneWeight + ";" + NL.MutationChangeWeights + ";" + NL.MutationAddNeuron + ";" + NL.MutationChangeOneBias + ";"
                     + NL.MutationChangeBias + ";" + NL.ChildrenByWave + ";" + NL.ChildrenInWave + ";" + NL.IgnoreCollision + ";" + NL.AddingWeightsCount + ";"
                     + NL.ChangeWeightSign + ";" + NL.ChildrenDifference + ";" + NL.Autosave + ";" + NL.AutosaveStep + ";" + NL.AutosaveName + ";"
                     + NL.UseMemoryWeight[0] + ";" + NL.UseMemoryWeight[1] + ";" + NL.UseNeuronsMemory[0] + ";" + NL.UseNeuronsMemory[1]);
                string aft = "";
                int i = 0;
                while (i < NL.AFT.Count)
                {
                    aft += NL.AFT[i];
                    if (i < NL.AFT.Count - 1)
                        aft += ";";
                    i++;
                }
                SC.WriteLine(aft);
                SC.Close();
            }
            Debug.Log("The NEAT settings saved. File name is: " + SettingsName);
        }
        else
            Debug.LogWarning("The NEAT settings not saved. File name not entered.");
    }

    private void Load()
    {
        if (SettingsName != "")
        {
            if (Directory.Exists(Application.dataPath + "/ANN/Settings"))
            {
                if (File.Exists(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbn"))
                {
                    StreamReader SR = File.OpenText(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbn");
                    string Version = SR.ReadLine();
                    if (Version == "ANN NEAT settings:")
                    {
                        string settings = SR.ReadLine();
                        string s = "";
                        int stage = 0;
                        int c = 0;
                        while (c < settings.Length)
                        {
                            if (settings[c] != ';' && c != settings.Length - 1)
                                s += settings[c];
                            else
                            {
                                if (c == settings.Length - 1)
                                    s += settings[c];
                                if (stage == 0)
                                    NL.ChanceCoefficient = Formulas.StringToFloat(s);
                                else if (stage == 1)
                                    NL.AmountOfChildren = Formulas.StringToInt(s);
                                else if (stage == 2)
                                    NL.Cross = Formulas.StringToBool(s);
                                else if (stage == 3)
                                    NL.PerceptronStart = Formulas.StringToBool(s);
                                else if (stage == 4)
                                    NL.MutationAddWeight = Formulas.StringToFloat(s);
                                else if (stage == 5)
                                    NL.MutationChangeOneWeight = Formulas.StringToFloat(s);
                                else if (stage == 6)
                                    NL.MutationChangeWeights = Formulas.StringToFloat(s);
                                else if (stage == 7)
                                    NL.MutationAddNeuron = Formulas.StringToFloat(s);
                                else if (stage == 8)
                                    NL.MutationChangeOneBias = Formulas.StringToFloat(s);
                                else if (stage == 9)
                                    NL.MutationChangeBias = Formulas.StringToFloat(s);
                                else if (stage == 10)
                                    NL.ChildrenByWave = Formulas.StringToBool(s);
                                else if (stage == 11)
                                    NL.ChildrenInWave = Formulas.StringToInt(s);
                                else if (stage == 12)
                                    NL.IgnoreCollision = Formulas.StringToBool(s);
                                else if (stage == 13)
                                    NL.AddingWeightsCount = Formulas.StringToInt(s);
                                else if (stage == 14)
                                    NL.ChangeWeightSign = Formulas.StringToFloat(s);
                                else if (stage == 15)
                                    NL.ChildrenDifference = Formulas.StringToFloat(s);
                                else if (stage == 16)
                                    NL.Autosave = Formulas.StringToBool(s);
                                else if (stage == 17)
                                    NL.AutosaveStep = Formulas.StringToInt(s);
                                else if (stage == 18)
                                    NL.AutosaveName = s;
                                else if (stage == 19)
                                    NL.UseMemoryWeight[0] = Formulas.StringToBool(s);
                                else if (stage == 20)
                                    NL.UseMemoryWeight[1] = Formulas.StringToBool(s);
                                else if (stage == 21)
                                    NL.UseNeuronsMemory[0] = Formulas.StringToFloat(s);
                                else if (stage == 22)
                                    NL.UseNeuronsMemory[1] = Formulas.StringToFloat(s);
                                s = "";
                                stage++;
                            }
                            c++;
                        }
                        settings = SR.ReadLine();
                        NL.AFT.Clear();
                        c = 0;
                        while (c < settings.Length)
                        {
                            if (settings[c] != ';' && c != settings.Length - 1)
                                s += settings[c];
                            else
                            {
                                if (c == settings.Length - 1)
                                    s += settings[c];
                                NL.AFT.Add(Formulas.StringToInt(s));
                                s = "";
                            }
                            c++;
                        }
                        SR.Close();
                        Debug.Log("The ANN NEAT settings loaded.");
                    }
                    else
                    {
                        SR.Close();
                        Debug.LogWarning("The ANN NEAT settings not loaded. An unsuitable version.");
                    }
                }
                else
                    Debug.LogWarning("The ANN NEAT settings not loaded. There is no such filename.");
            }
            else
                Debug.LogWarning("The ANN NEAT settings not loaded. Folder for the settings does not exist.");
        }
        else
            Debug.LogWarning("The ANN NEAT settings not loaded. Filename not entered.");
    }
}