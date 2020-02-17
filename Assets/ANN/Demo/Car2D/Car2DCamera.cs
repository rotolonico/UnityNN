using UnityEngine;

public class Car2DCamera : MonoBehaviour
{
    private float Distance;
    private Vector2 Speed;
    private Transform Target;
    private Vector2 TargetP;
    private float Accelerate;
    private float Turn;
    private float LifeTime;
    private float MaxLifeTime;
    void Update()
    {
        Distance = -Mathf.Infinity;
        Vector3 V = Vector3.zero;
        Transform T = null;
        foreach (Car2DControl CC in FindObjectsOfType<Car2DControl>())
            if (CC.Distance > Distance && !CC.Crash)
            {
                LifeTime = CC.LifeTime;
                MaxLifeTime = CC.MaxLifeTime + 10;
                if (MaxLifeTime > 180)
                    MaxLifeTime = 180;
                T = CC.transform;
                Distance = CC.Distance;
                Speed = CC.Speed;
                Accelerate = CC.Accelerate;
                Turn = CC.Turn;
            }
        if (Target != T && T != null)
        {
            Target = T;
            TargetP = Target.position - transform.position;
        }
        if (Target != null)
        {
            TargetP = Vector3.Lerp(TargetP, Vector2.zero, Time.deltaTime * 3F);
            transform.position = (Vector3)((Vector2)Target.position - TargetP) - Vector3.forward * 10;
        }
    }

    void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(10, 10, 200, 30), "Life time: " + LifeTime.ToString("F2") + " / " + MaxLifeTime.ToString("F2"));
        GUI.Label(new Rect(10, 50, 200, 30), "Speed: " + Speed.y.ToString("F2"));
        GUI.Label(new Rect(10, 70, 200, 30), "Drag: " + Speed.x.ToString("F2"));
        GUI.Label(new Rect(10, 90, 200, 30), "Distance:" + Distance.ToString("F2"));
        GUI.Label(new Rect(10, 110, 200, 30), "Accelerate:" + Accelerate.ToString("F2"));
        GUI.Label(new Rect(10, 130, 200, 30), "Turn:" + Turn.ToString("F2"));
        GUI.color = Color.white;
    }
}