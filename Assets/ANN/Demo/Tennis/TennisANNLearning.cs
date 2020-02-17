using UnityEngine;

public class TennisANNLearning : MonoBehaviour
{
    public GameObject StudentGO;
    public TennisANN Student;
    public string Crash;
    public string Life;

    private ANNLearnByNEATInterface ALBN;

    void Start()
    {
        ALBN = gameObject.AddComponent<ANNLearnByNEATInterface>();
        ALBN.Ann = Student.Ann;
        ALBN.NL.AmountOfChildren = 300;
        ALBN.NL.ChildrenInWave = 20;
        ALBN.NL.ChildrenDifference = 2F;
        ALBN.NL.ChanceCoefficient = 0.05F;
        ALBN.NL.StudentData(StudentGO, Student, "Ann", Student, Crash, Life);
    }
}
