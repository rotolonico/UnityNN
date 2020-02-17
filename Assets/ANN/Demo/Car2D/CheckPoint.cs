using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int Number;
    public int NumberLast;
    void OnTriggerEnter2D(Collider2D C)
    {
        if (C != null)
        {
            Car2DControl CC = C.GetComponent<Car2DControl>();
            if (CC != null)
            {
                if (CC.Checkpoint == 0 && Number == NumberLast)
                    CC.Crash = true;
                else if (Number >= CC.Checkpoint || (Number == 0 && CC.Checkpoint == NumberLast))
                {
                    if (Number > CC.Checkpoint || (Number == 0 && CC.Checkpoint == NumberLast))
                    {
                        CC.MaxLifeTime += 1.5F;
                    }
                    CC.Checkpoint = Number;
                }
                else
                    CC.Crash = true;
            }
        }
    }
}