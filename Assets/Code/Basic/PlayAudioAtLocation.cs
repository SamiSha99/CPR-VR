using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioAtLocation : MonoBehaviour
{
    public AudioClip audioClip;
    public GameObject atObject;
    [Tooltip("Requires atObject not being null, will set this sound as child, and follow around till it expires.")]
    public bool setObjectAsParent;
    [Range(0.01f, 1.0f)]
    public float volume = 1.0f;
    public AudioSource activeAudioSource;
    public void TriggerAudio()
    {
        if(audioClip == null) return;
        Vector3 pos = atObject == null ? gameObject.transform.position : atObject.transform.position;
        GameObject parent = setObjectAsParent ? atObject : null;
        activeAudioSource = Util.PlayClipAt(audioClip, pos, volume, parent);
    }
    public void TriggerAudio(AudioClip customClip)
    {
        audioClip = customClip;
        TriggerAudio();
    }

    public void SetVolume(float v) => volume = v;

    public bool IsPlaying()
    {
        return activeAudioSource != null && activeAudioSource.isPlaying;
    }
}
