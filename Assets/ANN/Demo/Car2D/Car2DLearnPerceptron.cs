using UnityEngine;

public class Car2DLearnPerceptron : MonoBehaviour
{
    public GameObject StudentGO;
    public Car2DPerceptron Student;
    public string Crash;
    public string Life;

    private PerceptronRandomGenerationInterface PBPI;

    void Start()
    {
        PBPI = StudentGO.gameObject.AddComponent<PerceptronRandomGenerationInterface>();
        PBPI.PCT = Student.PCT;
        PBPI.PLBRG.ChanceCoefficient = 0.1F;
        PBPI.PLBRG.ChildrenGradient = true;
        PBPI.PLBRG.AmountOfChildren = 100;
        PBPI.PLBRG.ChildrenInWave = 100;
        PBPI.PLBRG.ChildrenByWave = false;
        PBPI.PLBRG.StudentData(StudentGO, Student, "PCT", Student.Car, Crash, Life);
    }
}