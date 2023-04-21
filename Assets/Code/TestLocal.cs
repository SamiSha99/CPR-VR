using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
public class TestLocal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LocalizationManager.SetLanguageByCode("ar");
        Util.Print(LocalizationManager.GetAsset<AudioClip>("AudioTable", "AudioTest").name);
        Util.Print(LocalizationManager.GetText("TextTable", "exampleKey3"));
        LocalizationManager.SetLanguageByCode("en");
        Util.Print(LocalizationManager.GetAsset<AudioClip>("AudioTable", "AudioTest").name);
        Util.Print(LocalizationManager.GetText("TextTable", "exampleKey"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
