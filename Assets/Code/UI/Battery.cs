using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Battery : MonoBehaviour
{
    public GameObject batteryUIIcon;
    public TextMeshProUGUI batteryPrecentageText;
    float lastUpdate = -1;
    float batteryLevel = SystemInfo.batteryLevel;
    BatteryStatus batteryStatus = SystemInfo.batteryStatus;
    const float UPDATE_BATTERY_INTERVALS = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        lastUpdate = Time.timeSinceLevelLoad;
        UpdateBattery();
    }

    // Update is called once per frame
    void Update()
    {
        if(lastUpdate + UPDATE_BATTERY_INTERVALS > Time.timeSinceLevelLoad) return;
        UpdateBattery();
    }

    void UpdateBattery()
    {        
        batteryLevel = SystemInfo.batteryLevel;
        batteryStatus = SystemInfo.batteryStatus;
        
        //Util.Print<Battery>("Battery Amount: " + batteryLevel + " | Status: " + batteryStatus);
        lastUpdate = Time.timeSinceLevelLoad;
        //Util.Print<Battery>("Next update: " + (lastUpdate + UPDATE_BATTERY_INTERVALS));

        if(batteryUIIcon != null)
        {

        }
        
        if(batteryPrecentageText != null)
        {
            
        }
    }
}
