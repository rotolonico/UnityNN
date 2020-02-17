using UnityEngine;

public class Lesson4CowLearning : MonoBehaviour
{
    public TutorialCowControl TCС;
    public Lesson4TutorialCow TCN;

    private ANNLearnByNEATInterface NLI;
    void Start()
    {
        NLI = gameObject.AddComponent<ANNLearnByNEATInterface>();
        NLI.Ann = TCN.Ann;
        NLI.NL.AmountOfChildren = 100;
        NLI.NL.ChildrenByWave = false;
        NLI.NL.ChildrenInWave = 10;
        NLI.NL.StudentData(TCN.gameObject, TCN, "Ann", TCС, "Death", "LifeTime");
    }
}
