using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The interface of an artificial neural network.
/// </summary>
public class ANNInterface : MonoBehaviour
{
    /// <summary>
    /// Artificial neural network.
    /// </summary>
    public ANN Ann;
    private Rect WindowRectInterface = new Rect(0, Screen.height, 390, 245);
    private Rect WindowRectVisualization;
    private bool ShowVisualization = false;
    private bool Resizing = false;

    private bool VisualizationWeightsFade = false;
    private int VisualizationNeurons = 0;
    private float VisualizationWeightsWidth = 1;
    private string NEATName = "";
    private int ShowMemoryWeights = 0;
    private Texture AFV;

    private void Start()
    {
        if (gameObject.name == "StudentChild")
            Destroy(gameObject.GetComponent<ANNInterface>());
        else
        {
            WindowRectVisualization.x = Screen.width / 3;
            WindowRectVisualization.y = Screen.height / 3;
            WindowRectVisualization.width = Screen.width / 3;
            WindowRectVisualization.height = Screen.height / 3;
        }
    }

    private void OnGUI()
    {
        if (Ann != null)
        {
            WindowRectInterface = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), WindowRectInterface, WindowInterface, "ANN in " + transform.name);  // interface window
            if (ShowVisualization)
                WindowRectVisualization = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), WindowRectVisualization, WindowVisualization, "Visualization of ANN in " + transform.name);  // visualization window
        }
    }

    private void WindowInterface(int ID)                                                                                // interface window
    {
        if (WindowRectInterface.height == 65)                                                                           // small window
        {
            if (GUI.Button(new Rect(WindowRectInterface.width - 20, 0, 20, 20), "▄"))                                   // change window scale
            {
                WindowRectInterface.width = 390;
                WindowRectInterface.height = 245;
                WindowRectInterface.x -= 190;
            }
            ShowVisualization = InterfaceGUI.Button(1, 1, "Visualization OFF", "Visualization ON", ShowVisualization);  // ANN's visualization (ON / OFF)
        }
        else                                                                                                            // big window
        {
            if (GUI.Button(new Rect(WindowRectInterface.width - 20, 0, 20, 20), "-"))                                   // change window scale
            {
                WindowRectInterface.width = 200;
                WindowRectInterface.height = 65;
                WindowRectInterface.x = WindowRectInterface.x;
                WindowRectInterface.x += 190;
            }

            bool Redraw = false;
            Ann.AFT = InterfaceGUI.IntArrows(1, 1, ActivationFunctions.Names, Ann.AFT, ref Redraw);
            Ann.AFWM = InterfaceGUI.Button(1, 2, "Without Minus", "With Minus", Ann.AFWM, ref Redraw);
            Ann.AFS = InterfaceGUI.HorizontalSlider(1, 3, "Scale", Ann.AFS, 0.1F, 5F, ref Redraw);

            GUI.DrawTexture(new Rect(101, 28, 71, 25), AFV);

            InterfaceGUI.Info(1, 4, "Inputs", Ann.Input.Length);
            InterfaceGUI.Info(1, 5, "Outputs", Ann.Output.Length);
            InterfaceGUI.Info(1, 6, "Hidden neurons", Ann.Node.Count - Ann.Input.Length - Ann.Output.Length);
            InterfaceGUI.Info(1, 7, "Weights", Ann.Connection.Count);
            ShowVisualization = InterfaceGUI.Button(2, 1, "Visualization OFF", "Visualization ON", ShowVisualization);  // ANN's visualization (ON / OFF)

            NEATName = GUI.TextField(new Rect(200, 175, 180, 30), NEATName);
            if (NEATName=="")
                GUI.enabled = false;
            bool Save = false;
            Save = InterfaceGUI.MiddleButton(3, 7, "Save", "Save", Save);
            bool Load = false;
            Load = InterfaceGUI.MiddleButton(4, 7, "Load", "Load", Load);
            GUI.enabled = true;
            if (Save)
                Ann.Save(NEATName);
            else if (Load)
            {
                Ann.Load(NEATName);
                Redraw = true;
            }
            if (AFV == null || Redraw)
            {
                AFV = ActivationFunctions.ActivationFunctionView(Ann.AFT, Ann.AFS, Ann.AFWM);
                int i = 0;
                while (i < Ann.Input.Length + Ann.Output.Length)
                {
                    Ann.Node[i].AFT = Ann.AFT;
                    i++;
                }
            }
        }
        WindowRectInterface = InterfaceGUI.WindowControl(WindowRectInterface);
    }

    private void WindowVisualization(int ID)
    {
        if (WindowRectVisualization.width != Screen.width && WindowRectVisualization.height != Screen.height)           // change window scale
        {
            if (GUI.Button(new Rect(WindowRectVisualization.width - 40, 0, 20, 20), "▄"))
            {
                WindowRectVisualization.x = 0;
                WindowRectVisualization.y = 0;
                WindowRectVisualization.width = Screen.width;
                WindowRectVisualization.height = Screen.height;
            }
        }
        else
        {
            if (GUI.Button(new Rect(WindowRectVisualization.width - 40, 0, 20, 20), "_"))
            {
                WindowRectVisualization.x = Screen.width / 3;
                WindowRectVisualization.y = Screen.height / 3;
                WindowRectVisualization.width = Screen.width / 3;
                WindowRectVisualization.height = Screen.height / 3;
            }
        }

        if (GUI.Button(new Rect(WindowRectVisualization.width - 20, 0, 20, 20), "x"))                                   // close window
            ShowVisualization = false;

        Visualization(WindowRectVisualization.width, WindowRectVisualization.height);

        WindowRectVisualization = InterfaceGUI.WindowControl(WindowRectVisualization);

        Vector2 Mouse = GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
        Rect MouseZone = new Rect(WindowRectVisualization.width - 20, WindowRectVisualization.height - 20, 20, 20);
        Rect Resize = new Rect();
        if (Event.current.type == EventType.MouseDown && MouseZone.Contains(Mouse))
        {
            Resizing = true;
            Resize = new Rect(Mouse.x, Mouse.y, WindowRectVisualization.width, WindowRectVisualization.height);
        }
        else if (Event.current.type == EventType.MouseUp && Resizing)
        {
            Resizing = false;
        }
        else if (!Input.GetMouseButton(0))
        {
            Resizing = false;
        }
        else if (Resizing)
        {
            WindowRectVisualization.width = Mathf.Max(100, Resize.width + (Mouse.x - Resize.x));
            WindowRectVisualization.height = Mathf.Max(100, Resize.height + (Mouse.y - Resize.y));
            WindowRectVisualization.xMax = Mathf.Min(Screen.width, WindowRectVisualization.xMax);
            WindowRectVisualization.yMax = Mathf.Min(Screen.height, WindowRectVisualization.yMax);
        }
        if (WindowRectVisualization.width < 390)
            WindowRectVisualization.width = 390;
    }

    private void Visualization(float width, float height)
    {
        if (Ann != null)
        {
            Vector2 v = new Vector2(20, 30);
            width -= 40;
            height -= 80;

            if (VisualizationWeightsFade && GUI.Button(new Rect(10, 25, 85, 30), "Fade"))
                VisualizationWeightsFade = false;
            if (!VisualizationWeightsFade && GUI.Button(new Rect(10, 25, 85, 30), "Triange"))
                VisualizationWeightsFade = true;

            string VT = "Value";
            if (VisualizationNeurons == 1)
                VT = "Dot";
            else if (VisualizationNeurons == 2)
                VT = "Bias";
            else if (VisualizationNeurons == 3)
                VT = "Number";
            else if (VisualizationNeurons == 4)
                VT = "AFT";
            else if (VisualizationNeurons == 5)
                VT = "M-Weights";
            if (GUI.Button(new Rect(105, 25, 85, 30), VT))
                VisualizationNeurons++;
            if (VisualizationNeurons > 5)
                VisualizationNeurons = 0;

            VisualizationWeightsWidth = InterfaceGUI.HorizontalSlider(2, 1, "Weights width", VisualizationWeightsWidth, 0.1F, 10);

            // All weights / forward weights / Memory weights
            string M = "↔";
            if (ShowMemoryWeights == 1)
                M = "→";
            else if (ShowMemoryWeights == 2)
                M = "←";
            if (InterfaceGUI.SmallButton(2, 1, 1, M))
                ShowMemoryWeights++;
            if (ShowMemoryWeights > 2)
                ShowMemoryWeights = 0;

            TextAnchor SaveTA = GUI.skin.GetStyle("Label").alignment;
            if (VisualizationNeurons != 1)
                GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleCenter;

            if (Event.current.type == EventType.Repaint)
            {
                if (Ann.Connection.Count > 0)
                {
                    List<int> WL = new List<int>(Ann.Connection.Keys);
                    int w = 0;
                    while (w < Ann.Connection.Count)
                    {
                        int temp = WL[w];
                        Vector2 P1 = new Vector2(Ann.Node[Ann.Connection[temp].In].Position.x * width, Ann.Node[Ann.Connection[temp].In].Position.y * height + 40);
                        Vector2 P2 = new Vector2(Ann.Node[Ann.Connection[temp].Out].Position.x * width, Ann.Node[Ann.Connection[temp].Out].Position.y * height + 40);
                        if (ShowMemoryWeights == 0 || (Ann.Connection[temp].In < Ann.Input.Length && Ann.Connection[temp].IsMemory))
                        {
                            if (Ann.Connection[temp].Enable)
                            {
                                if (Ann.Connection[temp].In < Ann.Input.Length)
                                    DrawANNWeight.Line(P1 + v, P2 + v, VisualizationWeightsWidth * Ann.Connection[temp].Weight, Mathf.Abs(Formulas.ActivationFunction(Ann.Node[Ann.Connection[temp].In].Neuron * Ann.Connection[temp].Weight, Ann.AFS, Ann.AFWM)), VisualizationWeightsFade);
                                else
                                    DrawANNWeight.Sideline(P1 + v, P2 + v, VisualizationWeightsWidth * Ann.Connection[temp].Weight, Mathf.Abs(Formulas.ActivationFunction(Ann.Node[Ann.Connection[temp].In].Neuron * Ann.Connection[temp].Weight, Ann.AFS, Ann.AFWM)), VisualizationWeightsFade);
                            }
                            else
                            {
                                if (Ann.Connection[temp].In < Ann.Input.Length)
                                    DrawANNWeight.Line(P1 + v, P2 + v, VisualizationWeightsWidth * Ann.Connection[temp].Weight, VisualizationWeightsFade);
                                else
                                    DrawANNWeight.Sideline(P1 + v, P2 + v, VisualizationWeightsWidth * Ann.Connection[temp].Weight, VisualizationWeightsFade);
                            }
                        }
                        else if ((!Ann.Connection[temp].IsMemory && ShowMemoryWeights == 1) || (Ann.Connection[temp].IsMemory && ShowMemoryWeights == 2))
                        {
                            if (Ann.Connection[temp].Enable)
                            {
                                if (ShowMemoryWeights == 2 && Ann.Connection[temp].In >= Ann.Input.Length + Ann.Output.Length)
                                    DrawANNWeight.Sideline(P1 + v, P2 + v, VisualizationWeightsWidth * Ann.Connection[temp].Weight, Mathf.Abs(Formulas.ActivationFunction(Ann.Node[Ann.Connection[temp].In].Neuron * Ann.Connection[temp].Weight, Ann.AFS, Ann.AFWM)), VisualizationWeightsFade);
                                else
                                    DrawANNWeight.Line(P1 + v, P2 + v, VisualizationWeightsWidth * Ann.Connection[temp].Weight, Mathf.Abs(Formulas.ActivationFunction(Ann.Node[Ann.Connection[temp].In].Neuron * Ann.Connection[temp].Weight, Ann.AFS, Ann.AFWM)), VisualizationWeightsFade);
                            }
                            else
                            {
                                if (ShowMemoryWeights == 2 && Ann.Connection[temp].In >= Ann.Input.Length + Ann.Output.Length)
                                    DrawANNWeight.Sideline(P1 + v, P2 + v, VisualizationWeightsWidth * Ann.Connection[temp].Weight, VisualizationWeightsFade);
                                else
                                    DrawANNWeight.Line(P1 + v, P2 + v, VisualizationWeightsWidth * Ann.Connection[temp].Weight, VisualizationWeightsFade);
                            }
                        }
                        w++;
                    }
                    WL.Clear();
                }
            }
            if (Ann.Node.Count > 0)
            {
                List<int> NL = new List<int>(Ann.Node.Keys);
                int n = 0;
                while (n < Ann.Node.Count)
                {
                    int temp = NL[n];
                    if (VisualizationNeurons != 1)
                    {
                        string S = "";

                        if (VisualizationNeurons == 2)
                            S = Ann.Node[temp].Bias.ToString("f2");
                        else if (VisualizationNeurons == 0)
                            S = Ann.Node[temp].Neuron.ToString("f2");
                        else if (VisualizationNeurons == 3)
                            S = temp.ToString();
                        else if (VisualizationNeurons == 4)
                            S = Ann.Node[temp].AFT.ToString();
                        else if (VisualizationNeurons == 5)
                        {
                            if (Ann.Node[temp].UseMemory)
                                S = Ann.Node[temp].WeightMemory.ToString("f2");
                            else
                                S = "";
                        }
                        if (temp < Ann.Input.Length)
                            GUI.Label(new Rect(Ann.Node[temp].Position.x * width, Ann.Node[temp].Position.y * height + 50, 40, 40), S, InterfaceStyle.InputCell());
                        else if (temp < Ann.Input.Length + Ann.Output.Length)
                        {
                            if (Ann.Node[temp].UseMemory)
                                GUI.Label(new Rect(Ann.Node[temp].Position.x * width, Ann.Node[temp].Position.y * height + 50, 40, 40), S, InterfaceStyle.OutputMemoryCell());
                            else
                                GUI.Label(new Rect(Ann.Node[temp].Position.x * width, Ann.Node[temp].Position.y * height + 50, 40, 40), S, InterfaceStyle.OutputCell());
                        }
                        else
                        {
                            if (Ann.Node[temp].UseMemory)
                                GUI.Label(new Rect(Ann.Node[temp].Position.x * width, Ann.Node[temp].Position.y * height + 50, 40, 40), S, InterfaceStyle.HiddenMemoryCell());
                            else
                                GUI.Label(new Rect(Ann.Node[temp].Position.x * width, Ann.Node[temp].Position.y * height + 50, 40, 40), S, InterfaceStyle.HiddenCell());
                        }
                    }
                    else
                    {
                        float V = Mathf.Abs(Ann.Node[temp].Neuron);
                        float P = 10 + V * 30;
                        float C = 15 - V * 15;

                        if (temp < Ann.Input.Length)
                            GUI.Label(new Rect(Ann.Node[temp].Position.x * width + C, Ann.Node[temp].Position.y * height + 50 + C, P, P), "", InterfaceStyle.InputCell());
                        else if (temp < Ann.Input.Length + Ann.Output.Length)
                        {
                            if (Ann.Node[temp].UseMemory)
                                GUI.Label(new Rect(Ann.Node[temp].Position.x * width + C, Ann.Node[temp].Position.y * height + 50 + C, P, P), "", InterfaceStyle.OutputMemoryCell());
                            else
                                GUI.Label(new Rect(Ann.Node[temp].Position.x * width + C, Ann.Node[temp].Position.y * height + 50 + C, P, P), "", InterfaceStyle.OutputCell());
                        }
                        else
                        {
                            if (Ann.Node[temp].UseMemory)
                                GUI.Label(new Rect(Ann.Node[temp].Position.x * width + C, Ann.Node[temp].Position.y * height + 50 + C, P, P), "", InterfaceStyle.HiddenMemoryCell());
                            else
                                GUI.Label(new Rect(Ann.Node[temp].Position.x * width + C, Ann.Node[temp].Position.y * height + 50 + C, P, P), "", InterfaceStyle.HiddenCell());
                        }
                    }
                    n++;
                }
                NL.Clear();
            }
            if (VisualizationNeurons != 1)
                GUI.skin.GetStyle("Label").alignment = SaveTA;
        }
    }
}