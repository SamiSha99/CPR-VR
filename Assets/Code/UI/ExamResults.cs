using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using System.Linq;

public class ExamResults : MonoBehaviour
{
    public TextMeshProUGUI Title, MistakeTitle, Mistakes, FinalScoreText, ScoreValue, LeaveButton, NameText, PassText, PassResultText;
    public List<GameManager.ExamPenalty> MistakesLocalization;
    public GameObject MistakeChildTemplate, MistakesList;
    private string localizedMistakes;
    private float score;
    private bool passed;
    void Awake() => LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    void OnDestroy() => LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    public void ShowFinalScore(float score, List<GameManager.ExamPenalty> mistakeLocalizations, bool passed = false)
    {
        MistakesLocalization = mistakeLocalizations;
        this.score = score;
        this.passed = passed;
        LocalizeContent();
        gameObject.SetActive(true);
    }
    
    void LocalizeContent()
    {
        LocalizationHelper.LocalizeTMP("ExamResults.Results", Title);
        LocalizationHelper.LocalizeTMP("ExamResults.Mistakes", MistakeTitle);
        LocalizationHelper.LocalizeTMP("ExamResults.FinalScore", FinalScoreText);
        LocalizationHelper.LocalizeTMP("ExamResults.Leave", LeaveButton);
        LocalizationHelper.LocalizeTMP("ExamResults.Name", NameText);
        
        LocalizationHelper.LocalizeTMP("ExamResults.Passed", PassText);
        LocalizationHelper.LocalizeTMP("ExamResults." + (passed ? "Yes" : "No"), PassResultText);

        while(MistakesList.transform.childCount > 0) DestroyImmediate(MistakesList.transform.GetChild(0).gameObject);
        if(MistakesLocalization.Count <= 0)
            AddMistakeChild(new GameManager.ExamPenalty("ExamResults.Perfect"));
        else
        {    
            for(int i = 0; i < MistakesLocalization.Count; i++) AddMistakeChild(MistakesLocalization[i]);
            float mistakesScore = MistakesLocalization.Sum(x => x.penaltyAmount);
            // We don't want to recalculate the score, its already defined!
            if(score >= 100) score -= mistakesScore;
            GameManager._Instance.score = score;
        }
        LocalizationHelper.LocalizeTMP(string.Empty, ScoreValue);
        ScoreValue.text = $"{string.Format("{0:0.##}", score)}%";
        if(ScoreValue != null && score >= 100) ScoreValue.color = Color.green; // Wow! awesome!
    }

    void AddMistakeChild(GameManager.ExamPenalty penalty)
    {
        GameObject go = Instantiate(MistakeChildTemplate, MistakesList.transform);
        go.name = penalty.penaltyName;
        TextMeshProUGUI info = go.transform.FindComponent<TextMeshProUGUI>("MistakeInfo");
        TextMeshProUGUI times = go.transform.FindComponent<TextMeshProUGUI>("MistakeTimes");
        LocalizationHelper.LocalizeTMP(penalty.penaltyName, info);
        if(penalty.penaltyAmount > 0)
        {
            LocalizationHelper.LocalizeTMP(string.Empty, times);
            times.text = $"-{string.Format("{0:0.##}", penalty.penaltyAmount)}";
        }
        else
            times.text = "";       
    }    

    void OnLanguageChanged(Locale selectedLanguage)
    {
        LocalizeContent();
    }

    public void ReturnToMenu() => Util.LoadMenu();
}
