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
    public LocalizationManager _Instance;
    void Awake() => _Instance = this;
    void Start()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
        if(LocalizationHelper.UsingLanguage(0)) return;
        
        if(LocalizationHelper.UsingRightToLeftLanguage())
        {
            FlipUI();
        }

        //Util.Invoke(this, ()=>LocalizationHelper.SetLanguage(PlayerPrefs.GetInt()), 0.1f);
        //LocalizationHelper.SetLanguage("ar");
    }
    void OnDestroy() => LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    void OnEnable() => LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    void OnLanguageChanged(Locale selectedLanguage) => FlipUI();
    void FlipUI()
    {
        bool result = LocalizationHelper.UsingRightToLeftLanguage();
        if(UIFlipped == result) return;
        UIFlipped = result;
        //Debug.Log("Flipping =? " + UIFlipped);
        LocalizationHelper.FlipCanvas();
    }
}

