using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject XROriginMenu;
    public GameObject XROriginPlayer;
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

    public void OnQuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif 
    }

    // to-do: make it do something?
    public void OnSettingsButtonPressed()
    {

    }
}
