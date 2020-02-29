using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SliderRandomizer : MonoBehaviour
{
    private void Start()
    {
        var slider = GetComponent<Slider>();
        slider.value = RandomnessHandler.RandomMinMax(slider.minValue, slider.maxValue);
    }

    private IEnumerator Randomize()
    {
        yield return new WaitForSeconds(1);
    }
}
