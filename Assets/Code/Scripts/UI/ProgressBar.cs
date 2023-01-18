using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider _slider;
    public void UpdateProgressBar(float normalizedValue) => Mathf.Clamp(_slider.value = normalizedValue, _slider.minValue, _slider.maxValue);
    public void AddProgressBar(float normalizedValue) => Mathf.Clamp(_slider.value += normalizedValue, _slider.minValue, _slider.maxValue);
}
