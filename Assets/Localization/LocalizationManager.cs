using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
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
            FixArabicInTMPro(UsingLanguage("ar"));
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
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, key, null);
    }

    static public T GetAsset<T>(string table, string key) where T : UnityEngine.Object
    {
        return LocalizationSettings.AssetDatabase.GetLocalizedAsset<T>(table, key);
    }

    static public void FixArabicInTMPro(bool _enable)
    {
        foreach (ArabicFixerTMPRO fixers in Util.FindAllInScene<ArabicFixerTMPRO>())
            UnityEngine.Object.DestroyImmediate(fixers);
        foreach (TextMeshProUGUI meshpros in Util.FindAllInScene<TextMeshProUGUI>())
        {
            Util.Print("Adding to" + meshpros.gameObject.name);
            //meshpros.gameObject.AddComponent<ArabicFixerTMPRO>();
        }
    }
}
