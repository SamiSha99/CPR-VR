using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public bool useCentimeter = false;
    [Range(0,1)]
    public float textToSpeechVolume = 0.8f;
    public int loops = 3;

    const string OPTION_TTS_VOLUME = "OPTION_TTS_VOLUME";
    const string OPTION_CENTIMETERS = "OPTION_CENTIMETERS";
    const string OPTION_CPR_LOOPS = "OPTION_CPR_LOOPS";

    void Start()
    {
        LoadPrefs();
    }

    public void OnSettingAdjusted(GameObject OptionRow)
    {
        string optionaName = OptionRow.name;

        switch(optionaName)
        {
            case OPTION_TTS_VOLUME:
                Slider s = OptionRow.transform.FindComponent<Slider>("OptionConfig/Slider");
                PlayAudioAtLocation audioSample = OptionRow.transform.FindComponent<PlayAudioAtLocation>("OptionConfig/Slider");
                
                float volume = s.value;
                textToSpeechVolume = volume;
                audioSample.SetVolume(volume);
                if(audioSample.activeAudioSource != null)
                    audioSample.activeAudioSource.volume = volume;
                if(!audioSample.IsPlaying()) audioSample.TriggerAudio(LocalizationManager.GetAsset<AudioClip>("AudioTable", "AudioVolumeTest"));
                break;
    
            case OPTION_CENTIMETERS:
                Toggle t = OptionRow.transform.FindComponent<Toggle>("OptionConfig/Toggle");
                useCentimeter = t.isOn;
                break;

            case OPTION_CPR_LOOPS:
                TMP_Dropdown d = OptionRow.transform.FindComponent<TMP_Dropdown>("OptionConfig/Dropdown");
                loops = d.value;
                break;

            default:
                Debug.LogWarning("This option is not defined, what is this? Ignoring: " + optionaName);
                return;
        }
    }

    public void LoadPrefs(GameObject options = null)
    {
        useCentimeter = SettingsUtility.IsChecked(nameof(useCentimeter), false);
        textToSpeechVolume = PlayerPrefs.GetFloat(nameof(textToSpeechVolume), textToSpeechVolume);
        loops = PlayerPrefs.GetInt(nameof(loops), loops);
        if(options != null)
            UpdateOptionsUI(options);
    }

    void UpdateOptionsUI(GameObject options)
    {
        List<Transform> optionRows = new List<Transform>();

        options.transform.GetChildren(optionRows);
        foreach(Transform or in optionRows)
        {
            string optionaName = or.gameObject.name;
            GameObject optionRow = or.gameObject;
            switch(optionaName)
            {
                case OPTION_TTS_VOLUME:
                    Slider s = optionRow.transform.FindComponent<Slider>("OptionConfig/Slider");
                    s.value = textToSpeechVolume;
                    //s.SetValueWithoutNotify(textToSpeechVolume);
                    break;
    
                case OPTION_CENTIMETERS:
                    Toggle t = optionRow.transform.FindComponent<Toggle>("OptionConfig/Toggle");
                    t.isOn = useCentimeter;
                    break;

                case OPTION_CPR_LOOPS:
                    TMP_Dropdown d = optionRow.transform.FindComponent<TMP_Dropdown>("OptionConfig/Dropdown");
                    d.value = loops;
                    break;

                default:
                    Debug.LogWarning("This option is not defined, what is this? Ignoring: " + optionaName);
                return;
            }
        }
    }

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
