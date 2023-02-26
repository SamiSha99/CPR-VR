using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicTest : MonoBehaviour
{
    private AudioClip microphoneClip;
    void Start() => microphoneClip = Util.MicrophoneToAudioClip();
    void Update()
    {
        if(microphoneClip == null) return;
        float loudness = Util.GetLoudnessFromMicrophone(microphoneClip, 32) * 100;
        Util.Print("MicTest", "Loud: " + loudness); 
        if(loudness < 0.05f) loudness = 0;
        transform.localScale = Vector3.Lerp(new Vector3(1,1,1), new Vector3(3.0f,3.0f,3.0f), loudness);
    }
}
