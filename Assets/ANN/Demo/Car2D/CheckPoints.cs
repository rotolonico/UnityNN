using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    void Start()
    {
        int i = 0;
        while (i < transform.childCount)
        {
            CheckPoint CP = transform.GetChild(i).gameObject.AddComponent<CheckPoint>();
            CP.Number = i;
            CP.NumberLast = transform.childCount - 1;
            i++;
        }
    }
}