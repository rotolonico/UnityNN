using UnityEngine;
using System.IO;

/// <summary>
/// The interface of perceptron's random generation learning method.
/// </summary>
public class PerceptronRandomGenerationInterface : MonoBehaviour
{
    /// <summary>
    /// The perceptron.
    /// </summary>
    public Perceptron PCT;

    /// <summary>
    /// Perceptron' random generation learning method.
    /// </summary>
    public PerceptronLernByRandomGeneration PLBRG = new PerceptronLernByRandomGeneration();

    /// <summary>
    /// Is perceptron learning?
    /// </summary>
    public bool Learn = false;

    private Rect WindowRect = new Rect(Screen.width, Screen.height, 580, 305);

    private bool Settings = true;

    private string SettingsName = "";
    private void Start()
    {
        if (gameObject.name == "StudentChild")
            Destroy(gameObject.GetComponent<PerceptronRandomGenerationInterface>());
        else
        {
            if (PCT == null)
                Debug.LogWarning("The interface of random generation does not have the perceptron.");
        }
    }

    private void Update()
    {
        if (PCT != null && Learn)
            PLBRG.Learn(PCT);
        PerceptronInterface PI = gameObject.GetComponent<PerceptronInterface>();
        if (PI != null)
            if (PI.Reload)
                PLBRG.Reset();
    }

    private void OnGUI()
    {
        if (PCT != null)
        {
            WindowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), WindowRect, Window, "Random generation of " + transform.name);    // interface window
        }
    }

    private void Window(int ID)                                                                             // interface window
    {
        if (WindowRect.height == 65)                                                                // small window
        {
            if (GUI.Button(new Rect(WindowRect.width - 20, 0, 20, 20), "▄"))                        // change window scale
            {
                if (Settings)
                {
                    WindowRect.width = 580;
                    WindowRect.height = 305;
                    WindowRect.x = WindowRect.x - 380;
                }
                else
                {
                    WindowRect.width = 200;
                    WindowRect.height = 245;
                }
            }
        }
        else                                                                                        // big window
        {
            if (GUI.Button(new Rect(WindowRect.width - 20, 0, 20, 20), "-"))                        // change window scale
            {
                WindowRect.width = 200;
                WindowRect.height = 65;
                if (Settings)
                    WindowRect.x = WindowRect.x + 380;
            }

            if (GUI.Button(new Rect(WindowRect.width - 40, 0, 20, 20), "S"))                        // Settings on/off
            {
                if (Settings)
                {
                    WindowRect.width = 200;
                    WindowRect.height = 245;
                    WindowRect.x = WindowRect.x + 380;
                    Settings = false;
                }
                else
                {
                    WindowRect.width = 580;
                    WindowRect.height = 305;
                    WindowRect.x = WindowRect.x - 380;
                    Settings = true;
                }
            }

            if (Settings)
            {

                PLBRG.ChildrenDifference = InterfaceGUI.HorizontalSlider(2, 2, "Children difference", PLBRG.ChildrenDifference, 0.01F, 10F);
                PLBRG.ChildrenGradient = InterfaceGUI.Button(2, 3, "Children gradient OFF", "Children gradient ON", PLBRG.ChildrenGradient);

                PLBRG.GenerationEffect = InterfaceGUI.HorizontalSlider(2, 4, "Generation effect", PLBRG.GenerationEffect, 0F, 1F);
                PLBRG.GenerationSplashEffect = InterfaceGUI.HorizontalSlider(2, 5, "Splash effect coefficient", PLBRG.GenerationSplashEffect, 0F, 1F);
                PLBRG.ChanceCoefficient = InterfaceGUI.HorizontalSlider(1, 6, "Chance coefficient", PLBRG.ChanceCoefficient, 0F, 0.5F);
                PLBRG.ChangeWeightSign = InterfaceGUI.HorizontalSlider(3, 5, "Chance change sign", PLBRG.ChangeWeightSign, PLBRG.ChangeWeightSign * 100, 0F, 1F);

                if (Learn)
                    GUI.enabled = false;
                PLBRG.AmountOfChildren = InterfaceGUI.IntArrows(2, 1, "Children amount", PLBRG.AmountOfChildren, 1);

                if (PLBRG.AmountOfChildren > 1)
                {
                    if (PLBRG.AmountOfChildren != PLBRG.ChildrenInWave)
                        PLBRG.ChildrenByWave = InterfaceGUI.Button(3, 1, "Maximum in one time", "By waves", PLBRG.ChildrenByWave);
                    PLBRG.ChildrenInWave = InterfaceGUI.IntArrows(3, 2, "Children in wave", PLBRG.ChildrenInWave, 1, PLBRG.AmountOfChildren);
                }

                PLBRG.IgnoreCollision = InterfaceGUI.Button(3, 3, "Collision ON", "Collision OFF", PLBRG.IgnoreCollision);

                GUI.enabled = true;
                PLBRG.Autosave = InterfaceGUI.Button(3, 7, "Autosave OFF", "Autosave ON", PLBRG.Autosave);
                if (PLBRG.Autosave)
                {
                    PLBRG.AutosaveStep = InterfaceGUI.IntArrows(3, 8, "Autosave step", PLBRG.AutosaveStep, 1);
                    PLBRG.AutosaveName = GUI.TextField(new Rect(390, 265, 180, 30), PLBRG.AutosaveName);
                }
                if (!Learn)
                    PLBRG.ChildrenDifferenceAfterEffects = PLBRG.ChildrenDifference;

                SettingsName = GUI.TextField(new Rect(10, 235, 180, 30), SettingsName);
                if (InterfaceGUI.MiddleButton(1, 9, "Save"))
                    Save();
                if (InterfaceGUI.MiddleButton(2, 9, "Load"))
                    Load();

                InterfaceGUI.InfoF2(2, 6, "Children difference", PLBRG.ChildrenDifferenceAfterEffects);
            }
            else
            {
                if (!Learn)
                    PLBRG.ChildrenDifferenceAfterEffects = PLBRG.ChildrenDifference;
                InterfaceGUI.InfoF2(1, 6, "Children difference", PLBRG.ChildrenDifferenceAfterEffects);
            }
            InterfaceGUI.Info(1, 2, "Best generation", PLBRG.BestGeneration);
            InterfaceGUI.Info(1, 3, "Generation", PLBRG.Generation);
            InterfaceGUI.Info(1, 4, "Children", PLBRG.ChildrenInGeneration);
            InterfaceGUI.Info(1, 5, "Best longevity", PLBRG.BestLongevity);

            

            if (PLBRG.ChanceCoefficient != 0)
                InterfaceGUI.Info(1, 7, "Chance", PLBRG.Chance);
        }

        bool Activte = false;
        Learn = InterfaceGUI.Button(1, 1, "Learn OFF", "Learn ON", Learn, ref Activte);

        if (Activte && !Learn)
            PLBRG.StopLearn(PCT);

        if (WindowRect.x < 0)                                           //window restriction on the screen
            WindowRect.x = 0;
        else if (WindowRect.x + WindowRect.width > Screen.width)
            WindowRect.x = Screen.width - WindowRect.width;
        if (WindowRect.y < 0)
            WindowRect.y = 0;
        else if (WindowRect.y + WindowRect.height > Screen.height)
            WindowRect.y = Screen.height - WindowRect.height;

        GUI.DragWindow(new Rect(0, 0, WindowRect.width, 20));
    }

    private void Save()
    {
        if (SettingsName != "")
        {
            if (!Directory.Exists(Application.dataPath + "/ANN/Settings"))
            {
                Directory.CreateDirectory(Application.dataPath + "/ANN/Settings");
            }
            if (File.Exists(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbr"))
                File.Delete(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbr");

            if (!File.Exists(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbr"))
            {
                StreamWriter SC = File.CreateText(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbr");
                SC.WriteLine("Perceptron's random generation settings:");
                SC.WriteLine(PLBRG.ChanceCoefficient + ";" + PLBRG.AmountOfChildren + ";" + PLBRG.ChildrenDifference + ";" + PLBRG.ChildrenGradient + ";"
                    + PLBRG.GenerationEffect + ";" + PLBRG.GenerationSplashEffect + ";" + PLBRG.ChildrenByWave + ";" + PLBRG.ChildrenInWave + ";"
                    + PLBRG.IgnoreCollision + ";" + PLBRG.ChangeWeightSign + ";" + PLBRG.Autosave + ";" + PLBRG.AutosaveStep + ";" + PLBRG.AutosaveName);
                SC.Close();
            }
            Debug.Log("The perceptron's random generation settings saved. File name is: " + SettingsName);
        }
        else
            Debug.LogWarning("The perceptron's random generation settings not saved. File name not entered.");
    }

    private void Load()
    {
        if (SettingsName != "")
        {
            if (Directory.Exists(Application.dataPath + "/ANN/Settings"))
            {
                if (File.Exists(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbr"))
                {
                    StreamReader SR = File.OpenText(Application.dataPath + "/ANN/Settings/" + SettingsName + ".lbr");
                    string Version = SR.ReadLine();
                    if (Version == "Perceptron's random generation settings:")
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
                                    PLBRG.ChanceCoefficient = Formulas.StringToFloat(s);
                                else if (stage == 1)
                                    PLBRG.AmountOfChildren = Formulas.StringToInt(s);
                                else if (stage == 2)
                                    PLBRG.ChildrenDifference = Formulas.StringToFloat(s);
                                else if (stage == 3)
                                    PLBRG.ChildrenGradient = Formulas.StringToBool(s);
                                else if (stage == 4)
                                    PLBRG.GenerationEffect = Formulas.StringToFloat(s);
                                else if (stage == 5)
                                    PLBRG.GenerationSplashEffect = Formulas.StringToFloat(s);
                                else if (stage == 6)
                                    PLBRG.ChildrenByWave = Formulas.StringToBool(s);
                                else if (stage == 7)
                                    PLBRG.ChildrenInWave = Formulas.StringToInt(s);
                                else if (stage == 8)
                                    PLBRG.IgnoreCollision = Formulas.StringToBool(s);
                                else if (stage == 9)
                                    PLBRG.ChangeWeightSign = Formulas.StringToFloat(s);
                                else if (stage == 10)
                                    PLBRG.Autosave = Formulas.StringToBool(s);
                                else if (stage == 11)
                                    PLBRG.AutosaveStep = Formulas.StringToInt(s);
                                else if (stage == 12)
                                    PLBRG.AutosaveName = s;
                                s = "";
                                stage++;
                            }
                            c++;
                        }
                        SR.Close();
                        Debug.Log("The Perceptron's random generation settings loaded.");
                    }
                    else
                    {
                        SR.Close();
                        Debug.LogWarning("The Perceptron's random generation settings not loaded. An unsuitable version.");
                    }
                }
                else
                    Debug.LogWarning("The Perceptron's random generation settings not loaded. There is no such filename.");
            }
            else
                Debug.LogWarning("The Perceptron's random generation settings not loaded. Folder for the settings does not exist.");
        }
        else
            Debug.LogWarning("The Perceptron's random generation settings not loaded. Filename not entered.");
    }
}