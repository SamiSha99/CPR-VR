using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicSensor : MonoBehaviour
{
    public GameObject TalkingImage;
    public ProgressBar micProgressBar;

    void Awake() => XREvents.onTalking += OnTalking; 
    void OnDestroy() => XREvents.onTalking -= OnTalking; 

    void OnTalking(float loud, float loudNormal)
    {
        // tell no one hush
        ShowTalkImage(loudNormal >= 0.8f);
        micProgressBar.SetProgressBar(loudNormal);
    }

    void ShowTalkImage(bool enable = false) => TalkingImage?.SetActive(enable);
    
}
