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
        //LocalizationHelper.SetLanguage("ar");
    }
    void OnDestroy() => LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    void OnEnable() => LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    void OnLanguageChanged(Locale selectedLanguage) => FlipUI();
    void FlipUI()
    {
        if(UIFlipped == LocalizationHelper.UsingRightToLeftLanguage()) return;
        UIFlipped = !UIFlipped;
        Debug.Log("Flipping: " + UIFlipped);
        LocalizationHelper.FlipCanvas();
    }
}

