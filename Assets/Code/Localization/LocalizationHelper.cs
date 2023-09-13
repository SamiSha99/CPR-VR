using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.ResourceManagement.AsyncOperations;

/*
* Hello, coders who's reading this file. 👋
* Made by Sami Shakkour (Alias: SamiSha), please note that this is a very basic setup and also has another extension (ArabicFixer)
* meaning that the content is psuedo hard coded for some, unless i come up with a different solution... some day.
* This file simplify a lot of the Localization stuff for me at least, you are free to use anything for whatever reason.
*/
public static class LocalizationHelper
{
    /// <summary>Set the language using the locale ID. 0 = English, 1 = Arabic.</summary>
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
    /// <summary>Set the language using locale culture code. en = English, ar = Arabic.</summary>
    static public bool SetLanguage(string code)
    {
        if(code == "") return false;

        foreach(Locale locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if(locale.Identifier.Code != code) continue;
            LocalizationSettings.SelectedLocale = locale;
            return true;
        }
        Util.Print("The code " + code + " is not found in the AvailableLocales", Util.PrintType.Warn);
        return false;
    }
    /// <summary>Returns true if we are using this langauge by index</summary>
    static public bool UsingLanguage(int id) 
    { 
        List<Locale> locales = GetAvailableLanguages();
        if(locales.Count -1 < id) return false;
        return locales[id] == LocalizationSettings.SelectedLocale; 
    }
    /// <summary>Returns true if the selected language matches the culture code</summary>
    static public bool UsingLanguage(string langCode) 
    { 
        return LocalizationSettings.SelectedLocale.Identifier.Code == langCode; 
    }

    /// <summary>Returns true if the langauge we are using is considered "Right to Left".</summary>
    static public bool UsingRightToLeftLanguage()
    {
        if(UsingLanguage("ar")) return true;
        return false;
    }
    /// <summary>Returns all languages as a list of Locale(s).</summary>
    static public List<Locale> GetAvailableLanguages() 
    { 
        return LocalizationSettings.AvailableLocales.Locales;
    }
    /// <summary>Returns all langagues as a list of names.</summary>
    static public List<string> GetAvailableLanguagesAsNames() 
    { 
        return GetAvailableLanguages().Select(l => l.Identifier.CultureInfo.DisplayName).ToList();
    }
    /// <summary>Returns the currently selected language. Literally LocalizationSettings.SelectedLocale.</summary>
    static public Locale GetSelectedLanguage() => LocalizationSettings.SelectedLocale;
    
    /// <summary>Returns the index of the currently selected language.</summary>
    static public int GetSelectedLanguageIndex()
    {
        List<Locale> languages = GetAvailableLanguages();
        for(int i = 0; i < languages.Count; i++)
        {
            if(languages[i] != LocalizationSettings.SelectedLocale) continue;
            return i;
        }
        return 0;
    }
    /// <summary>Returns a localized string of the passed localization command. Command format: "TableName.KeyName".</summary>
    static public string GetText(string localizationCommand)
    {
        if(localizationCommand.Contains(" ") || localizationCommand.Count(f => f == '.') != 1) return localizationCommand;
        string[] text = localizationCommand.Split(".");
        if(text.Length != 2 || text.Contains(" ")) return localizationCommand;
        if(char.IsNumber(text[0][0]) || char.IsNumber(text[1][0])) return null;
        
        return GetText(text[0], text[1]);
    }
    /// <summary>Returns a localized string of the inputted table and key.</summary>
    static public string GetText(string table, string key)
    {
        //string text = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(table, key, null).Result;
        string text = LocalizationSettings.StringDatabase.GetLocalizedString(table, key, null);
        return text;
    }
    /// <summary>Returns a localized asset of the passed localization command. Command format: "TableName.KeyName".</summary>
    static public T GetAsset<T>(string localizationCommand) where T : UnityEngine.Object
    {
        if(localizationCommand.Contains(" ") || localizationCommand.Count(f => f == '.') != 1) return null;
        string[] text = localizationCommand.Split(".");
        if(text.Length != 2) return null;
        return GetAsset<T>(text[0], text[1]);
    }
    /// <summary>Returns a localized asset of the inputted table and key.</summary>
    static public T GetAsset<T>(string table, string key) where T : UnityEngine.Object
    {
        return LocalizationSettings.AssetDatabase.GetLocalizedAsset<T>(table, key);
    }
    /// <summary>Returns the font asset for the language.</summary>
    static public TMP_FontAsset GetFontAsset()
    {
        return GetAsset<TMP_FontAsset>("FontTable.FontAsset");
    }
    ///<summary>Localize the command, applies to the TextMeshPro component, updates the font asset and returns the text result.</summary> 
    static public string LocalizeTMP(string localizationCommand, TextMeshProUGUI textMeshPro = null)
    {
        string text = GetText(localizationCommand);
        if(textMeshPro == null) return text;
        textMeshPro.font = GetFontAsset();
        textMeshPro.text = text;

        RectTransform rt = textMeshPro.gameObject.GetComponent<RectTransform>();
        bool rtl = UsingRightToLeftLanguage();
        
        Canvas parentCanvas = textMeshPro?.gameObject.GetComponentInParent<Canvas>()?.rootCanvas;
        if(parentCanvas != null)
        {
            RectTransform rtParent = parentCanvas.gameObject.GetComponent<RectTransform>();
            if(rtParent?.localScale.x > 0 && rtl || rtParent?.localScale.x < 0 && !rtl) FlipComponent(rtParent);
        }
        if(rtl && rt?.localScale.x > 0 || !rtl && rt?.localScale.x < 0)
            rt.localScale = Vector3.Scale(rt.localScale, new Vector3(-1,1,1));

        if(textMeshPro.horizontalAlignment != HorizontalAlignmentOptions.Center)
            if(textMeshPro.horizontalAlignment != HorizontalAlignmentOptions.Right && rtl)
                textMeshPro.horizontalAlignment = HorizontalAlignmentOptions.Right;
            else if(textMeshPro.horizontalAlignment != HorizontalAlignmentOptions.Left && !rtl)
                textMeshPro.horizontalAlignment = HorizontalAlignmentOptions.Left; 
        
        if(!UsingLanguage("ar")) return text;
        if(textMeshPro.gameObject.HasComponent<UIScript>(out UIScript s) && s.dontFixArabic) return text;

        if(textMeshPro.gameObject.HasComponent<ArabicFixerTMPRO>(out ArabicFixerTMPRO af)) af.Rebuild();
        else textMeshPro.gameObject.AddComponent<ArabicFixerTMPRO>().Rebuild();

        return text;
    }

    /// <summary>Fixes formatting for all TMP_Text used in the current active scene, runs on language swapping.</summary>
    static public void FixArabicFormat(bool _enable)
    {
        foreach (TMP_Text TMPtext in Util.FindAllInScene<TMP_Text>(true))
        {
            if(TMPtext.gameObject.HasComponent<UIScript>(out UIScript s) && s.dontFixArabic) continue;
            if(TMPtext.gameObject.HasComponent<ArabicFixerTMPRO>(out ArabicFixerTMPRO af))
            {
                af.enabled = _enable;
                continue;
            }
            if(!_enable) continue;
            TMPtext.gameObject.AddComponent<ArabicFixerTMPRO>();
        }
    }
    
    // Behaves like a toggle!
    /// <summary>Flips the Canvas's X scale, this is done to support languages that are written from Right to Left.</summary>
    static public void FlipCanvas()
    {
        // making everything "Right to Left"
        foreach (Canvas c in Util.FindAllInScene<Canvas>(true))
        {
            if(!c.gameObject.HasComponent<RectTransform>(out RectTransform canvasTransform)) continue;
            FlipComponent(canvasTransform);
        }

        bool rtl = UsingRightToLeftLanguage();
        foreach (UIScript s in Util.FindAllInScene<UIScript>(true))
        {
            if(!s.supportsRightToLeftUI) continue;
            if(!s.gameObject.HasComponent<RectTransform>(out RectTransform canvasTransform)) continue;
            FlipComponent(canvasTransform);
            s.isRight = rtl;
        }

        FixArabicFormat(UsingLanguage("ar"));
    }

    static public void FlipComponent(RectTransform rect)
    {
        bool scriptableUI = rect.gameObject.HasComponent<UIScript>(out UIScript s);
        if(scriptableUI && !s.supportsRightToLeftUI) return;
        rect.localScale = Vector3.Scale(rect.localScale, new Vector3(-1, 1, 1));
    }
}

