using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioAtLocation : MonoBehaviour
{
    public AudioClip audioClip;
    public GameObject atObject;
    public bool setObjectAsParent;
    [Range(0.01f, 1.0f)]
    public float volume = 1.0f;
    public void TriggerAudio()
    {
        if(audioClip == null) return;
        if(atObject == null) return;
        Util.PlayClipAt(audioClip, atObject.transform.position);
    }
}
