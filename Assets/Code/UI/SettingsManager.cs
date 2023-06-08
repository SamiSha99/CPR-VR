using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class SettingsManager : MonoBehaviour
{
    public bool useCentimeter = false;
    [Range(0,1)]
    public float textToSpeechVolume = 0.8f;
    public int loops = 3;
    public int languageIndex;
    public GameObject optionsGameObject;
    public TextMeshProUGUI SettingsTitle;
    const string OPTION_TTS_VOLUME = "OPTION_TTS_VOLUME";
    const string OPTION_CENTIMETERS = "OPTION_CENTIMETERS";
    const string OPTION_CPR_LOOPS = "OPTION_CPR_LOOPS";
    const string OPTION_LANGUAGE = "OPTION_LANGUAGE";
    void Start()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
        if(SettingsTitle != null)
            LocalizationHelper.LocalizeTMP("MainMenu.Settings", SettingsTitle);
        LoadPrefs(optionsGameObject);
    }

    public void OnSettingAdjusted(GameObject OptionRow)
    {
        string optionaName = OptionRow.name;

        switch(optionaName)
        {
            case OPTION_TTS_VOLUME:
                PlayAudioAtLocation audioSample = OptionRow.transform.FindComponent<PlayAudioAtLocation>("OptionConfig/Slider");
                float volume = FindOptionConfig<Slider>(OptionRow, nameof(Slider)).value;
                textToSpeechVolume = volume;
                audioSample.SetVolume(volume);
                if(audioSample.activeAudioSource != null)
                    audioSample.activeAudioSource.volume = volume;
                if(!audioSample.IsPlaying())
                {
                    AudioClip testVolumeClip = LocalizationHelper.GetAsset<AudioClip>("AudioTable", "AudioVolumeTest");
                    if(testVolumeClip != null)
                        audioSample.TriggerAudio(testVolumeClip);
                    else
                        audioSample.TriggerAudio();

                }
                break;
    
            case OPTION_CENTIMETERS:
                useCentimeter = FindOptionConfig<Toggle>(OptionRow, nameof(Toggle)).isOn;
                break;

            case OPTION_CPR_LOOPS:
                loops = FindOptionConfig<TMP_Dropdown>(OptionRow, "Dropdown").value;
                break;
            case OPTION_LANGUAGE:
                languageIndex = FindOptionConfig<TMP_Dropdown>(OptionRow, "Dropdown").value;
                LocalizationHelper.SetLanguage(languageIndex);
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
        //languageIndex = PlayerPrefs.SetLanguage();
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
            TextMeshProUGUI tmpSettingName = or.FindComponent<TextMeshProUGUI>("OptionName");
            string localizationCommand = "";
            GameObject optionRow = or.gameObject;
            switch(optionaName)
            {
                case OPTION_TTS_VOLUME:
                    localizationCommand = "MainMenu.SpeechVolume";
                    Slider s = FindOptionConfig<Slider>(optionRow, nameof(Slider)); //optionRow.transform.FindComponent<Slider>("OptionConfig/Slider");
                    s.SetValueWithoutNotify(textToSpeechVolume);
                    break;
    
                case OPTION_CENTIMETERS:
                    localizationCommand = "MainMenu.SwapInches";
                    Toggle t = FindOptionConfig<Toggle>(optionRow, nameof(Toggle)); //optionRow.transform.FindComponent<Toggle>("OptionConfig/Toggle");
                    t.isOn = useCentimeter;
                    break;

                case OPTION_CPR_LOOPS:
                    localizationCommand = "MainMenu.CPRLoops";
                    TMP_Dropdown d = FindOptionConfig<TMP_Dropdown>(optionRow, "Dropdown");
                    d.value = loops;
                    break;
                
                case OPTION_LANGUAGE:
                    localizationCommand = "MainMenu.Language";
                    TMP_Dropdown dropDownLang = FindOptionConfig<TMP_Dropdown>(optionRow, "Dropdown");
                    dropDownLang.ClearOptions();
                    dropDownLang.AddOptions(LocalizationHelper.GetAvailableLanguagesAsNames());
                    dropDownLang.value = LocalizationHelper.GetSelectedLanguageIndex();
                    break;
                
                default:
                    Debug.LogWarning("This option is not defined, what is this? Ignoring: " + optionaName);
                    return;
            }
            LocalizationHelper.LocalizeTMP(localizationCommand, tmpSettingName);
        }
    }

    public void SaveChanges()
    {
        PlayerPrefs.SetInt(nameof(useCentimeter), Util.BoolToInt(useCentimeter));
        PlayerPrefs.SetFloat(nameof(textToSpeechVolume), textToSpeechVolume);
        PlayerPrefs.SetInt(nameof(loops), loops);
        PlayerPrefs.SetInt(nameof(languageIndex), languageIndex);
        PlayerPrefs.Save();
    }
    public void ResetToDefault()
    {
        PlayerPrefs.DeleteAll();
    }

    //optionRow.transform.FindComponent<TMP_Dropdown>("OptionConfig/Dropdown");
    private T FindOptionConfig<T>(GameObject optionRow, string goName) where T : UnityEngine.Component
    {
        return optionRow.transform.FindComponent<T>("OptionConfig/"+goName);
    }

    void OnLanguageChanged(Locale selectedLocale)
    {
        if(SettingsTitle != null)
            LocalizationHelper.LocalizeTMP("MainMenu.Settings", SettingsTitle);
        LoadPrefs(optionsGameObject);
    }
}
