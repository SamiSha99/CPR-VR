using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using System;

public class MistakePrompt : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    private string recievedLocalization;
    void Awake()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(Locale locale)
    {
        if(recievedLocalization == "") return;
        DoLocalization(recievedLocalization);   
    }

    public void DoLocalization(string localization)
    {
        LocalizationHelper.LocalizeTMP(localization, textMeshPro);
        recievedLocalization = localization;
    }
}
