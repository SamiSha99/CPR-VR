using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using TMPro;

public class LocalizationManager : MonoBehaviour
{
    public bool UIFlipped;
    public static LocalizationManager _Instance;
    void Awake() 
    {
        _Instance = this;
    }
    void Start()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
        int index = PlayerPrefs.GetInt(nameof(SettingsManager.languageIndex), 0);
        LocalizationHelper.SetLanguage(index);
        Util.Print("Setting the language (locale index): " + index);
        FlipUI();
    }
    void OnDestroy() => LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    void OnEnable() => LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    void OnLanguageChanged(Locale selectedLanguage) => FlipUI();
    public void FlipUI()
    {
        bool result = LocalizationHelper.UsingRightToLeftLanguage();
        if(UIFlipped == result) return;
        UIFlipped = result;
        //Debug.Log("Flipping =? " + UIFlipped);
        LocalizationHelper.FlipCanvas();
    }
}

