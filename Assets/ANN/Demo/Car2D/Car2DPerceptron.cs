using UnityEngine;

public class Car2DPerceptron : MonoBehaviour
{
    public Car2DControl Car;
    public Perceptron PCT = new Perceptron();

    void Start()
    {
        int[] NIHL = new int[2];
        NIHL[0] = 22;
        NIHL[1] = 22;
        PCT.CreatePerceptron(5, 1, false, true, 11, NIHL, 2);
        PerceptronInterface PI = gameObject.AddComponent<PerceptronInterface>();
        PI.PCT = PCT;
    }

    void Update()
    {
        if (Car == null)
            Car = gameObject.GetComponent<Car2DControl>();
        else
        {
            Formulas.FromArray(Car.Sensors, PCT.Input);
            int B = 0;
            if (PCT.B)
                B = 1;
            PCT.Input[PCT.Input.Length - 2 - B] = Car.Speed.y / 122.4F;
            PCT.Input[PCT.Input.Length - 1 - B] = Car.Speed.x / 122.4F;
            PCT.Solution();
            Car.Accelerate = PCT.Output[0];
            Car.Turn = PCT.Output[1];
        }
    }
}