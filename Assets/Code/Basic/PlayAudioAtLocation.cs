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
        Vector3 pos = atObject == null ? gameObject.transform.position : atObject.transform.position;
        GameObject parent = setObjectAsParent ? atObject : null;
        Util.PlayClipAt(audioClip, pos, volume, parent);
    }
}
