using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BR_Slider : MonoBehaviour
{
    private Slider _slider;

    // Start is called before the first frame update
    void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetSliderValues(float cur, float max, float min)
    {
        _slider.maxValue = max;
        _slider.minValue = min;
        _slider.value = cur;
    }

    public void SetSliderValues(float cur)
    {
        _slider.value = cur;
    }

}
