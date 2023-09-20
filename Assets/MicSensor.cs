using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MicSensor : MonoBehaviour
{
    public GameObject TalkingImage, TextPrompt;
    public ProgressBar micProgressBar;

    void Awake() => XREvents.onTalking += OnTalking;
    void OnDestroy() => XREvents.onTalking -= OnTalking; 

    public void SetText(string localizationKey)
    {
        if(localizationKey == "") localizationKey = "Other.MicSensorDefault";
        
        TextMeshProUGUI tmp = TextPrompt?.GetComponent<TextMeshProUGUI>();
        if(tmp == null) return; 
        LocalizationHelper.LocalizeTMP(localizationKey, tmp);
    }

    void OnTalking(float loud, float loudNormal)
    {
        // dead zone
        micProgressBar.SetProgressBar(loudNormal <= 0.025f ? 0.0f : loudNormal);
    }
}
