using UnityEngine;
public class Lesson4TutorialCow : MonoBehaviour
{
    private TutorialCowControl THC;     //Cow control
    public ANN Ann = new ANN();         //ANN
    private ANNInterface NI;            //ANN interface

    void Start()
    {
        //Find cow control
        THC = gameObject.GetComponent<TutorialCowControl>();

        //Create ANN
        Ann.Create(3, 2);

        //Add ANN interface to game object & add ANN to interface
        NI = gameObject.AddComponent<ANNInterface>();
        NI.Ann = Ann;
    }

    void Update()
    {
        //Convert vaule
        Ann.Input[0] = THC.AngleToFood / 180F;      //Work with angles. Min vaule = -180, max vaule = 180
        Ann.Input[1] = THC.DistanceToFood / 41F;    //Work with distance. Max vaule = 41
        Ann.Input[2] = THC.Satiety / 50F;           //Work with satiety. Min vaule = 0, max vaule = 50
        Ann.Solution();                             //ANN solution
        //For this tutorial not need to convert vaule
        THC.Turn = Ann.Output[0];
        THC.Move = Ann.Output[1];
    }
}