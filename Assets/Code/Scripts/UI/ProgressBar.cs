using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider _slider;
    public void SetProgressBar(float value) => Mathf.Clamp(_slider.value = value, _slider.minValue, _slider.maxValue);
    public void AddProgressBar(float value) => Mathf.Clamp(_slider.value += value, _slider.minValue, _slider.maxValue);
    // Set Min/Max Values of the progress bar beyond 0 - 1, call this without any parameters to reset to 0 - 1
    public void SetProgressBarClampValues(float min = 0, float max = 1)
    {
        _slider.minValue = min;
        _slider.maxValue = max;
    }

    // Returns between Min - Max, otherwise returns 0 - 1 if normalized = true (like a precentage betweeb 0 - 1)
    public float GetProgressBarValue(bool normalized = false)
    {
        return !normalized ? _slider.value : Mathf.Clamp01((_slider.value - _slider.minValue)/(_slider.maxValue - _slider.minValue));
    }
}
