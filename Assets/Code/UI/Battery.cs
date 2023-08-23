using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Battery : UIScript
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

        lastUpdate = Time.timeSinceLevelLoad;
        
#if UNITY_EDITOR

        batteryLevel = Random.value;
        if(batteryPrecentageText != null) batteryPrecentageText.text = "idk%";//$"{Mathf.RoundToInt(batteryLevel * 100)}%";
        batteryBar.SetProgressBar(batteryLevel);

#else

        batteryLevel = SystemInfo.batteryLevel;
        batteryStatus = SystemInfo.batteryStatus;

        if(batteryLevel != -1)
        {
            if(batteryPrecentageText != null) batteryPrecentageText.text = $"{Mathf.RoundToInt(batteryLevel * 100)}%";
            batteryBar.SetProgressBar(batteryLevel);
        }

#endif
    }
}
