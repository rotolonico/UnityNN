using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderRandomizer : MonoBehaviour
{
    private void Start()
    {
        var slider = GetComponent<Slider>();
        slider.SetValueWithoutNotify(RandomnessHandler.RandomMinMax(slider.minValue, slider.maxValue));
    }

    private IEnumerator Randomize()
    {
        yield return new WaitForSeconds(1);
        
    }
}
