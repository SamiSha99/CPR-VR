using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public bool useCentimeter = false;
    [Range(0,1)]
    public float textToSpeechVolume = 0.8f;
    public int loops = 3;

    public void SaveChanges()
    {
        PlayerPrefs.SetInt(nameof(useCentimeter), Util.BoolToInt(useCentimeter));
        PlayerPrefs.SetFloat(nameof(textToSpeechVolume), textToSpeechVolume);
        PlayerPrefs.SetInt(nameof(loops), loops);
        PlayerPrefs.Save();
    }
    public void ResetToDefault()
    {
        PlayerPrefs.DeleteAll();
    }
}
