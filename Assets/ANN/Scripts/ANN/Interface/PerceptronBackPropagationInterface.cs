using System.Collections;
using UnityEngine;

/// <summary>
/// The interface of perceptron's backpropagation learning method.
/// </summary>
public class PerceptronBackPropagationInterface : MonoBehaviour
{
    /// <summary>
    /// The perceptron.
    /// </summary>
    public Perceptron PCT;

    /// <summary>
    /// Array of tasks.
    /// </summary>
    public float[][] Task;

    /// <summary>
    /// Array of answers.
    /// </summary>
    public float[][] Answer;

    /// <summary>
    /// Is perceptron learning?
    /// </summary>
    public bool Learn = false;

    /// <summary>
    /// Perceptron's backpropagation learning method.
    /// </summary>
    public PerceptronLernByBackPropagation PLBBP = new PerceptronLernByBackPropagation();

    private bool ModificateStartWeights = false;
    private Rect WindowRect = new Rect(Screen.width, Screen.height, 390, 245);

    private bool ShowDiagramsWindow;
    private Rect WindowRectDiagrams;
    private bool WindowRectDiagramsResizing = false;

    private bool ShowMaxErrorDiagram = true;
    private bool ShowAccuracyDiagram = true;
    private void Start()
    {
        if (PCT == null)
            Debug.LogWarning("The interface of backpropagation does not have the perceptron.");

        WindowRectDiagrams.x = Screen.width / 3;
        WindowRectDiagrams.y = Screen.height / 3;
        WindowRectDiagrams.width = Screen.width / 3;
        WindowRectDiagrams.height = Screen.height / 3;
    }

    private void Update()
    {
        PerceptronInterface PI = gameObject.GetComponent<PerceptronInterface>();        // check if this GO have perceptron interface

        if (Learn && PCT != null && Answer != null)
        {
            if (Task == null)
                PLBBP.Learn(PCT, Answer[0]);                                            // perceptron's learning
            else
                PLBBP.Learn(PCT, Task, Answer);                                         // perceptron's learning
            Learn = !PLBBP.Learned;
        }

        if (PI != null)
            if (PI.Reload)
            {
                PLBBP.LearnIteration = 0;
                PLBBP.ModificateStartWeights(PCT, ModificateStartWeights);
                if (PLBBP.MaxErrorDiagram != null)
                    PLBBP.MaxErrorDiagram.Clear();
                if (PLBBP.AccuracyDiagram != null)
                    PLBBP.AccuracyDiagram.Clear();
            }
    }

    private void OnGUI()
    {
        if (PCT != null)
        {
            WindowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), WindowRect, Window, "Back propagation of " + transform.name);    // interface window
            if (ShowDiagramsWindow)
                WindowRectDiagrams = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), WindowRectDiagrams, WindowMaxErrorDiagram, "Diagrams of " + transform.name);   // visualization window
        }
    }

    private void Window(int ID)                                                                             // interface window
    {
        if (WindowRect.height == 65)                                                                // small window
        {
            if (GUI.Button(new Rect(WindowRect.width - 20, 0, 20, 20), "▄"))                        // change window scale
            {
                WindowRect.width = 390;
                WindowRect.height = 245;
                WindowRect.x = WindowRect.x - 190;
            }

            Learn = InterfaceGUI.Button(1, 1, "Learn OFF", "Learn ON", Learn);                              // start learn
        }
        else                                                                                        // big window
        {
            if (GUI.Button(new Rect(WindowRect.width - 20, 0, 20, 20), "-"))                        // change window scale
            {
                WindowRect.width = 200;
                WindowRect.height = 65;
                WindowRect.x = WindowRect.x + 200;
            }
            if (Learn)
                GUI.enabled = false;
            bool Temp = false;
            ModificateStartWeights = InterfaceGUI.Button(1, 1, "Weights from -0.5 to 0.5", "Mod Weights", ModificateStartWeights, ref Temp);
            if (Temp)
                PLBBP.ModificateStartWeights(PCT, ModificateStartWeights);

            PLBBP.ShuffleSamples = InterfaceGUI.Button(1, 2, "Samples one by one", "Shuffle samples", PLBBP.ShuffleSamples);
            GUI.enabled = true;

            if (Learn && Task == null)
                GUI.enabled = false;
            PLBBP.LearningSpeed = InterfaceGUI.HorizontalSlider(1, 3, "Learning speed", PLBBP.LearningSpeed, 1, 1800);
            GUI.enabled = true;
            PLBBP.LearningRate = InterfaceGUI.HorizontalSlider(1, 4, "Learning rate", PLBBP.LearningRate, 0F, 1F);
            if (Learn && Task == null)
                GUI.enabled = false;
            PLBBP.DesiredMaxError = InterfaceGUI.HorizontalSlider(1, 5, "Desired max error", PLBBP.DesiredMaxError, 0F, 1F);
            GUI.enabled = true;

            PLBBP.WithError = InterfaceGUI.Button(1, 6, "All outputs", "Errored outputs", PLBBP.WithError);

            Learn = InterfaceGUI.Button(1, 7, "Learn OFF", "Learn ON", Learn);                                         // start learning

            InterfaceGUI.Info(2, 1, "Iteration", PLBBP.LearnIteration);
            if (Task != null)
            {
                InterfaceGUI.Info(2, 2, "Max error", PLBBP.MaxError);
                if (PLBBP.AccuracyDiagram != null)
                    if (PLBBP.AccuracyDiagram.Count > 0)
                        InterfaceGUI.Info(2, 3, "Accuracy in %", (float)PLBBP.AccuracyDiagram[PLBBP.AccuracyDiagram.Count - 1] * 100);
                if (Task != null && InterfaceGUI.Button(2, 4, "Diagrams"))
                {
                    if (ShowDiagramsWindow)
                        ShowDiagramsWindow = false;
                    else
                        ShowDiagramsWindow = true;
                }
            }
        }

        GUI.DragWindow(new Rect(0, 0, WindowRect.width, 20));

        if (WindowRect.x < 0)                                           //window restriction on the screen
            WindowRect.x = 0;
        else if (WindowRect.x + WindowRect.width > Screen.width)
            WindowRect.x = Screen.width - WindowRect.width;
        if (WindowRect.y < 0)
            WindowRect.y = 0;
        else if (WindowRect.y + WindowRect.height > Screen.height)
            WindowRect.y = Screen.height - WindowRect.height;
    }

    private void WindowMaxErrorDiagram(int ID)
    {
        if (WindowRectDiagrams.width == Screen.width && WindowRectDiagrams.height == Screen.height)   // change window scale
        {
            if (GUI.Button(new Rect(WindowRectDiagrams.width - 40, 0, 20, 20), "_"))
            {
                WindowRectDiagrams.x = Screen.width / 3;
                WindowRectDiagrams.y = Screen.height / 3;
                WindowRectDiagrams.width = Screen.width / 3;
                WindowRectDiagrams.height = Screen.height / 3;
            }
        }
        else
        {
            if (GUI.Button(new Rect(WindowRectDiagrams.width - 40, 0, 20, 20), "▄"))
            {
                WindowRectDiagrams.x = 0;
                WindowRectDiagrams.y = 0;
                WindowRectDiagrams.width = Screen.width;
                WindowRectDiagrams.height = Screen.height;
            }
        }
        if (GUI.Button(new Rect(WindowRectDiagrams.width - 20, 0, 20, 20), "x"))                // close window
        {
            ShowDiagramsWindow = false;
        }

        ShowMaxErrorDiagram = InterfaceGUI.Button(1, 1, "Max error OFF", "Max error ON", ShowMaxErrorDiagram);
        ShowAccuracyDiagram = InterfaceGUI.Button(2, 1, "Accuracy OFF", "Accuracy ON", ShowAccuracyDiagram);

        GetDiargams();

        if (WindowRectDiagrams.x < 0)                                                          //window restriction on the screen
            WindowRectDiagrams.x = 0;
        else if (WindowRectDiagrams.x + WindowRectDiagrams.width > Screen.width)
            WindowRectDiagrams.x = Screen.width - WindowRectDiagrams.width;
        if (WindowRectDiagrams.y < 0)
            WindowRectDiagrams.y = 0;
        else if (WindowRectDiagrams.y + WindowRectDiagrams.height > Screen.height)
            WindowRectDiagrams.y = Screen.height - WindowRectDiagrams.height;

        GUI.DragWindow(new Rect(0, 0, WindowRectDiagrams.width, 20));

        Vector2 Mouse = GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
        Rect MouseZone = new Rect(WindowRectDiagrams.width - 20, WindowRectDiagrams.height - 20, 20, 20);
        Rect Resize = new Rect();
        if (Event.current.type == EventType.MouseDown && MouseZone.Contains(Mouse))
        {
            WindowRectDiagramsResizing = true;
            Resize = new Rect(Mouse.x, Mouse.y, WindowRectDiagrams.width, WindowRectDiagrams.height);
        }
        else if (Event.current.type == EventType.MouseUp && WindowRectDiagramsResizing)
        {
            WindowRectDiagramsResizing = false;
        }
        else if (!Input.GetMouseButton(0))
        {
            WindowRectDiagramsResizing = false;
        }
        else if (WindowRectDiagramsResizing)
        {
            WindowRectDiagrams.width = Mathf.Max(100, Resize.width + (Mouse.x - Resize.x));
            WindowRectDiagrams.height = Mathf.Max(100, Resize.height + (Mouse.y - Resize.y));
            WindowRectDiagrams.xMax = Mathf.Min(Screen.width, WindowRectDiagrams.xMax);
            WindowRectDiagrams.yMax = Mathf.Min(Screen.height, WindowRectDiagrams.yMax);
        }
        if (WindowRectDiagrams.width < 390)
            WindowRectDiagrams.width = 390;
    }

    private void GetDiargams()
    {
        Vector2 v = new Vector2(20, 60);

        float width = WindowRectDiagrams.width - 40;
        float height = WindowRectDiagrams.height - 80;

        DrawANNWeight.Line(v, v + new Vector2(0, height), 1, Color.white);
        DrawANNWeight.Line(v + new Vector2(0, height), v + new Vector2(width, height), 1, Color.white);

        DrawANNWeight.Line(v, v + new Vector2(width, 0), 1, Color.grey);
        DrawANNWeight.Line(v + new Vector2(width, 0), v + new Vector2(width, height), 1, Color.grey);


        //DrawANNWeight.Line(v + new Vector2(0, height * 0.2F), v + new Vector2(width, height * 0.2F), 1, Color.white);

        if (PLBBP.MaxErrorDiagram != null && (ShowMaxErrorDiagram || ShowAccuracyDiagram))
        {
            float MEK = 1;
            if (PCT.AFWM)
                MEK = 2F;
            if (ShowMaxErrorDiagram)
            {
                float MEL = height - height * PLBBP.DesiredMaxError / MEK;
                DrawANNWeight.Line(v + new Vector2(0, MEL), v + new Vector2(width, MEL), 1, Color.magenta);
            }
            int c = PLBBP.MaxErrorDiagram.Count;
            if (c > 1)
            {
                float W = width / (c - 1);
                float k = (c-1) / width;
                if (k < 1)
                    k = 1;

                Vector2 ME1 = v + new Vector2(0, height - height * (float)PLBBP.MaxErrorDiagram[0] / MEK);
                Vector2 AC1 = v + new Vector2(0, height - height * (float)PLBBP.AccuracyDiagram[0]);
                float i = 1;
                while (i < c)
                {
                    if (ShowMaxErrorDiagram)
                    {
                        Vector2 ME2 = v + new Vector2(W * i, height - height * (float)PLBBP.MaxErrorDiagram[(int)i] / MEK);
                        DrawANNWeight.Line(ME1, ME2, 1, Color.red);
                        ME1 = ME2;
                    }
                    if (ShowAccuracyDiagram)
                    {
                        Vector2 AC2 = v + new Vector2(W * i, height -  height * (float)PLBBP.AccuracyDiagram[(int)i]);
                        DrawANNWeight.Line(AC1, AC2, 1, Color.blue);
                        AC1 = AC2;
                    }
                    i += k;
                }
                if (ShowMaxErrorDiagram)
                    DrawANNWeight.Line(ME1, v + new Vector2(width, height - height * (float)PLBBP.MaxErrorDiagram[c - 1] / MEK), 1, Color.red);
                if (ShowAccuracyDiagram)
                    DrawANNWeight.Line(AC1, v + new Vector2(width, height - height * (float)PLBBP.AccuracyDiagram[c - 1]), 1, Color.blue);
            }
        }
    }
    private void GetDiargam()
    {

    }
}