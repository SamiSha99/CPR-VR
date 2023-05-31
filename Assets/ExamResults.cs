using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class ExamResults : MonoBehaviour
{
    public TextMeshProUGUI Title, MistakeTitle, Mistakes, FinalScoreText, ScoreValue, LeaveButton;
    public List<string> MistakesLocalization;
    private string localizedMistakes;
    private float score, maxScore;
    void Awake() => LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    void OnDestroy() => LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    public void ShowFinalScore(float score, float maxScore, List<string> mistakeLocalizations)
    {
        MistakesLocalization = mistakeLocalizations;
        this.score = score;
        this.maxScore = maxScore;
        LocalizeContent();
        gameObject.SetActive(true);
    }

    void LocalizeMistakes(List<string> mistakeLocalizations)
    {
        MistakesLocalization = mistakeLocalizations;
        localizedMistakes = "";
        for(int i = 0; i < MistakesLocalization.Count; i++)
        {
            localizedMistakes += $"{i + 1}) " + LocalizationHelper.GetText(MistakesLocalization[i]);
            localizedMistakes += "<br>";
        }
    }
    
    void LocalizeContent()
    {
        LocalizationHelper.LocalizeTMP("ExamResults.Results", Title);
        LocalizationHelper.LocalizeTMP("ExamResults.Mistakes", MistakeTitle);
        LocalizationHelper.LocalizeTMP("ExamResults.FinalScore", FinalScoreText);
        LocalizationHelper.LocalizeTMP("ExamResults.Leave", LeaveButton);
        
        localizedMistakes = "";
        if(MistakesLocalization.Count <= 0)
        {
            LocalizationHelper.LocalizeTMP("ExamResults.Perfect", Mistakes);
            ScoreValue.color = Color.green;
        }
        else
        {
            for(int i = 0; i < MistakesLocalization.Count; i++)
            {
                localizedMistakes += $"{i + 1}) " + LocalizationHelper.GetText(MistakesLocalization[i]);
                localizedMistakes += "<br>";
            }
            LocalizationHelper.LocalizeTMP(localizedMistakes, Mistakes);
        }
        LocalizationHelper.LocalizeTMP($"{score}%", ScoreValue);
        Util.Print("FINAL SCORE: " + ScoreValue.text + " | Score: " + score);
    }

    void OnLanguageChanged(Locale selectedLanguage)
    {
        LocalizeContent();
    }

    public void ReturnToMenu() => Util.LoadMenu();
}
