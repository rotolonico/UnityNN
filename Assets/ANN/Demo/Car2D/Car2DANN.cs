using UnityEngine;

public class Car2DANN : MonoBehaviour
{
    public Car2DControl Car;
    public ANN Ann = new ANN();

    void Start()
    {
        Ann.Create(true, 11, 2);
        ANNInterface NI = gameObject.AddComponent<ANNInterface>();
        NI.Ann = Ann;
    }

    void Update()
    {
        if (Car == null)
            Car = gameObject.GetComponent<Car2DControl>();
        else
        {
            Formulas.FromArray(Car.Sensors, Ann.Input);
            Ann.Input[Ann.Input.Length - 2] = Car.Speed.y / 122.4F;
            Ann.Input[Ann.Input.Length - 1] = Car.Speed.x / 122.4F;
            Ann.Solution();
            Car.Accelerate = Ann.Output[0];
            Car.Turn = Ann.Output[1];
        }
    }
}