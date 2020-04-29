using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrengthMeter : MonoBehaviour
{
    public Slider slider;

    public void SetMaxValue(float value) {
        slider.maxValue = value;
        slider.value = value;
    }
    public void SetSlider (float value) {
        slider.value = value;
    }
}
