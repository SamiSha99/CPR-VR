using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Battery : MonoBehaviour
{
    public TextMeshProUGUI batteryPrecentageText;
    public ProgressBar batteryBar;
    float lastUpdate = -1;
    float batteryLevel = -1;
    BatteryStatus batteryStatus;
    const float UPDATE_BATTERY_INTERVALS = 3.0f;
    void Start() => UpdateBattery();
    void Update()
    {
        if(lastUpdate + UPDATE_BATTERY_INTERVALS > Time.timeSinceLevelLoad) return;
        UpdateBattery();
    }
    void UpdateBattery()
    {        
        batteryLevel = SystemInfo.batteryLevel;
        batteryStatus = SystemInfo.batteryStatus;
        
        lastUpdate = Time.timeSinceLevelLoad;

        gameObject.SetActive(batteryLevel != -1);
        batteryPrecentageText.gameObject.SetActive(batteryLevel != -1);

        if(batteryLevel != -1)
        {
            if(batteryPrecentageText != null) batteryPrecentageText.text = $"{Mathf.RoundToInt(batteryLevel * 100)}%";
            batteryBar.SetProgressBar(batteryLevel);
        }
    }
}
