using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsUtility
{
    public static bool IsChecked(string key, bool? defaultValue = null)
    {
        int fallbackValue = defaultValue != null ? Util.BoolToInt((bool)defaultValue) : 0;
        return Util.IntToBool(PlayerPrefs.GetInt(key, fallbackValue));
    }

    public static bool ShouldUseCentimeter() { return IsChecked(nameof(SettingsManager.useCentimeter), false); }
}
