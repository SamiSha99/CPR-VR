using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsUtility
{
    public static bool IsChecked(string key, bool? defaultValue = null)
    {
        int fallbackValue = defaultValue != null ? Util.BoolToInt((bool)defaultValue) : 0;
        int res = PlayerPrefs.GetInt(key, fallbackValue);
        return Util.IntToBool(res);
    }
}
