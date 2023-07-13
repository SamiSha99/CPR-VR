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
        if(audioClip == null)
        {
            Util.Print("Audio Clip is set to null or wasn't defined!!!", Util.PrintType.Warn);
            return;
        }

        Vector3 pos = atObject == null ? gameObject.transform.position : atObject.transform.position;
        GameObject parent = setObjectAsParent ? atObject : null;
        activeAudioSource = Util.PlayClipAt(audioClip, pos, volume, parent);
    }
    public void TriggerAudio(AudioClip customClip)
    {
        audioClip = customClip;
        TriggerAudio();
    }

    // Localized VA Audio should follow the same rules as the settings and takes into the player's camera as the center position
    public void TriggerAudio(string localizationString)
    {
        audioClip = LocalizationHelper.GetAsset<AudioClip>(localizationString);
        SetVolume(PlayerPrefs.GetFloat(nameof(SettingsManager.textToSpeechVolume), 0.8f));
        atObject = Util.GetPlayer().GetPlayerCameraObject();
        TriggerAudio();
    }

    public void SetVolume(float v) => volume = v;

    public bool IsPlaying()
    {
        return activeAudioSource != null && activeAudioSource.isPlaying;
    }
}
