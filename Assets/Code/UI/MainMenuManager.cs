using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI title, practiceButton, examButton, videoButton, quitButton;
    void Start()
    {

        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
        Util.Invoke(this, () => LocalizeButtons(), 0.01f);
    }
    //void OnEnable() => LocalizeButtons();
    
    public void OnPracticeButtonPressed()
    {
        PlayerPrefs.SetInt("isExam", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("VR CPR");
    }

    public void OnExamButtonPressed()
    {
        PlayerPrefs.SetInt("isExam", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("VR CPR");
    }

    public void OnVideoButtonPressed()
    {
        SceneManager.LoadScene("VR 360 Video");
    }

    public void OnQuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif 
    }

    // to-do: make it do something? // to-do i haven't make it do something :(
    public void OnSettingsButtonPressed()
    {

    }

    void LocalizeButtons()
    {
        LocalizationHelper.LocalizeTMP("MainMenu.Title", title);
        LocalizationHelper.LocalizeTMP("MainMenu.Practice", practiceButton);
        LocalizationHelper.LocalizeTMP("MainMenu.Exam", examButton);
        LocalizationHelper.LocalizeTMP("MainMenu.Video", videoButton);
        LocalizationHelper.LocalizeTMP("MainMenu.Quit", quitButton);
    }

    void OnLanguageChanged(Locale selectedLocale) => LocalizeButtons();
}
