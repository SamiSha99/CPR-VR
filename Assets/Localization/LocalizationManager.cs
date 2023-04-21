using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using ArabicSupport;
using TMPro;

public static class LocalizationManager
{
    static public bool SetLanguage(int locale_ID)
    {
        if(LocalizationSettings.AvailableLocales.Locales.Count - 1 < locale_ID)
        {
            Util.Print("Could not find a language locale ID of [" + locale_ID + "], wrong index!");
            return false;
        }
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[locale_ID];
        return true;
    }

    static public bool SetLanguageByCode(string code)
    {
        if(code == "") return false;

        foreach(Locale locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if(locale.Identifier.Code != code) continue;
            LocalizationSettings.SelectedLocale = locale;
            FixArabicFormat(UsingLanguage("ar"));
            return true;
        }
        Util.Print("The code " + code + " is not found in the AvailableLocales");
        return false;
    }

    static public bool UsingLanguage(string langCode)
    {
        return LocalizationSettings.SelectedLocale.Identifier.Code == langCode;
    }

    // Gets a localized text
    // table = name of the table
    // key = the localized key
    static public string GetText(string table, string key)
    {
        string text = LocalizationSettings.StringDatabase.GetLocalizedString(table, key, null);
        return text;
    }

    static public T GetAsset<T>(string table, string key) where T : UnityEngine.Object
    {
        return LocalizationSettings.AssetDatabase.GetLocalizedAsset<T>(table, key);
    }

    static public void FixArabicFormat(bool _enable)
    {
        //Array.ForEach(Util.FindAllInScene<TMPro.TextMeshProUGUI>(), x => x.font = null);
        Array.ForEach(Util.FindAllInScene<ArabicFixerTMPRO>(), x => x.enabled = _enable);
    }
}
