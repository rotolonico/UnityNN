using UnityEngine;

public class Car2DLearnANN : MonoBehaviour
{
    public GameObject StudentGO;
    public Car2DANN Student;
    public string Crash;
    public string Life;

    private ANNLearnByNEATInterface NLBEI;

    void Start()
    {
        NLBEI = StudentGO.gameObject.AddComponent<ANNLearnByNEATInterface>();
        NLBEI.Ann = Student.Ann;
        NLBEI.NL.AmountOfChildren = 100;
        NLBEI.NL.ChildrenInWave = 20;
        NLBEI.NL.ChildrenByWave = false;
        NLBEI.NL.StudentData(StudentGO, Student, "Ann", Student.Car, Crash, Life);
    }
}