using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreListenerPauser : MonoBehaviour
{
    public List<AudioSource> audioSources = new List<AudioSource>();
    void Awake()
    {
        if(audioSources == null || audioSources.Count <= 0) return;
        audioSources.ForEach(audioSource => audioSource.ignoreListenerPause = true);
    }
}
