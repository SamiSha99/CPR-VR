using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using TMPro;
public class PauseMenu : MonoBehaviour
{
    public TextMeshProUGUI title, menuButton, backButton;

    void Awake()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
        GameMenuManager.OnMenuButtonPressed += OnPauseButtonsPressed;
        //OnLanguageChanged(null);
        gameObject.SetActive(false);
    }

    void Start()
    {
        Localize();
    }

    void OnDestroy()
    {
        GameMenuManager.OnMenuButtonPressed -= OnPauseButtonsPressed;
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    }

    public void OnBackClicked()
    {
        gameObject.SetActive(false);
        if(GameMenuManager._Instance.IsPaused())
            GameMenuManager._Instance.UnPause();
    }

    public void OnQuitToMenu()
    {
        if(GameMenuManager._Instance.IsPaused())
            GameMenuManager._Instance.UnPause();
        Util.LoadMenu();
    }

    void OnLanguageChanged(Locale l)
    {
        Localize();
    }

    void Localize()
    {
        LocalizationHelper.LocalizeTMP("Pause.Title", title);
        LocalizationHelper.LocalizeTMP("Pause.menuButton", menuButton);
        LocalizationHelper.LocalizeTMP("Pause.backButton", backButton);
        if(LocalizationHelper.UsingRightToLeftLanguage())
        {
            gameObject.GetComponent<RectTransform>().localScale = Vector3.Scale(gameObject.GetComponent<RectTransform>().localScale, new Vector3(-1,1,1));
            Util.Print("Flipping CANVAS");
        }
    }

    void OnPauseButtonsPressed(bool pause)
    {
        if(!pause) 
            OnBackClicked();
        else
            gameObject.SetActive(true);
        //Util.Print("OnPause Event: " + pause);
    }

}
