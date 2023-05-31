using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance;
    public List<Object> _TutorialQuestsLine;
    private List<Object> default_TutotrialQuestsLine;
    [Header("EXAM MANAGER")]
    // Exam
    public bool isExam;
    public bool completed;
    [Tooltip("The exam's content step by step, each step is evaulated.")]
    public List<Object> _ExamQuestsLine; // We do this step by step to examinate how fast/slow and effecient the player is and we add score
    public List<string> _ExamPenalty;
    public ExamResults resultCanvas;
    [Tooltip("These tasks are repeated continuously X times, depeding on Repeatable Amount, also evaulated")]
    public List<Object> _RepeatableQuestLine;
    [Min(1)]
    public int _RepeatableAmount = 1;
    private List<Object> default_ExamQuestsLine;
    public float score, maxScore;
    public GameEventCommand OnModuleProgressed;

    const string TUTORIAL_EVENT = "_Tutorial";
    const string EXAM_EVENT = "_Exam";

    void Awake() => _Instance = this;
    void Start()
    {
        //LocalizationHelper.SetLanguage("ar");
        if(Util.IsInMainMenu()) return;
        
        isExam = SettingsUtility.IsChecked(nameof(isExam), false);

        default_TutotrialQuestsLine = new List<Object>(_TutorialQuestsLine);
        default_ExamQuestsLine = new List<Object>(_ExamQuestsLine);
        _RepeatableAmount = Mathf.Max(1, _RepeatableAmount);

        if(isExam)
        {
            if(default_ExamQuestsLine.Count <= 0) return;
            score = maxScore;
            BuildExam();
            InstigateNextExamObject();
        }
        else
        {
            if(default_TutotrialQuestsLine.Count <= 0) return;
            InstigateNextTutorialObject();
        }
    }

    void Update()
    {
        
    }

    public void InstigateNextTutorialObject()
    {
        string command = "";
        if(OnGameComplete()) return;
        
        switch(_TutorialQuestsLine[0])
        {
            case Quest q:
                QuestManager._Instance.BeginQuest(q);
                command = q.questCommand;
                break;
            case AudioClip ac:
                GameObject head = Util.GetPlayer().GetPlayerCameraObject();
                AudioClip localizedAudio = LocalizationHelper.GetAsset<AudioClip>("TutorialAudio." + ac.name);
                if(localizedAudio == null) localizedAudio = ac; // cannot be found, use the english one
                Util.PlayClipAt(localizedAudio, head.transform.position, PlayerPrefs.GetFloat(nameof(SettingsManager.textToSpeechVolume), 1.0f), head);
                Util.Invoke(this, () => InstigateNextTutorialObject(), localizedAudio.length + 0.25f);
                command = ac.name;
                break;
        }
        OnModuleProgressed.TriggerEvent(command + TUTORIAL_EVENT);
        OnModuleProgressed.TriggerEvent(command);
        _TutorialQuestsLine.RemoveAt(0);
    }

    public void BuildExam()
    {

    }

    public void InstigateNextExamObject()
    {
        string command = "";
        if(OnGameComplete()) return;
        switch(_ExamQuestsLine[0])
        {
            case Quest q:
                QuestManager._Instance.BeginQuest(q);
                command = q.questCommand;
                break;
            case AudioClip ac:
                GameObject head = Util.GetPlayer().GetPlayerCameraObject();
                Util.PlayClipAt(ac, head.transform.position, PlayerPrefs.GetFloat(nameof(SettingsManager.textToSpeechVolume), 1.0f), head);
                Util.Invoke(this, () => InstigateNextTutorialObject(), ac.length + 0.25f);
                command = ac.name;
                break;
        }
        OnModuleProgressed.TriggerEvent(command + EXAM_EVENT);
        OnModuleProgressed.TriggerEvent(command);
        _ExamQuestsLine.RemoveAt(0);
    }

    public bool IsComplete()
    {
        if(isExam && _ExamQuestsLine.Count <= 0) return true;
        if(!isExam && _TutorialQuestsLine.Count <= 0) return true;
        return false;
    }

    bool OnGameComplete()
    {
        if(completed) return true;
        if(isExam && _ExamQuestsLine.Count > 0) return false;
        if(!isExam && _TutorialQuestsLine.Count > 0) return false;
        completed = true;

        if(isExam)
        {
            ShowFinalScore();
            Util.Invoke(this, () => Util.LoadMenu(), 20.0f);
        }
        else
            Util.Invoke(this, () => Util.LoadMenu(), 5.0f);
        return true;
    }

    private void ShowFinalScore()
    {
        if(resultCanvas != null)
        {
            resultCanvas.ShowFinalScore(score, maxScore, _ExamPenalty);
        }
    }

    public void AddExamPenalty(string mistakeLocalization)
    {
        if(_ExamPenalty.Contains(mistakeLocalization)) return;
        _ExamPenalty.Add(mistakeLocalization);    
    }
    public void AddExamPenalty(List<string> mistakeLocalizations)
    {
        foreach(string m in mistakeLocalizations)
        {
            if(_ExamPenalty.Contains(m)) continue;
            _ExamPenalty.Add(m);
        }
    }

    const int REDUCE_AFTER_SECONDS = 3;

    public void AdjustScore(float timeTaken, float averageTime, float accumulatedPenalties = 0)
    {
        if(timeTaken > averageTime)
        {
            accumulatedPenalties += GetTimePenalty(timeTaken, averageTime);
            AddExamPenalty("Exam.TimePenalty");
        }
        score = Mathf.Clamp(score - accumulatedPenalties, 0, maxScore);
        Util.Print("CURRENT SCORE: " + score + " | Lost: " + accumulatedPenalties);
    }

    private int GetTimePenalty(float timeTaken, float averageTime)
    {
        timeTaken -= timeTaken % REDUCE_AFTER_SECONDS; // remove leftovers
        timeTaken -= averageTime;
        timeTaken /= REDUCE_AFTER_SECONDS;
        timeTaken = Mathf.Clamp(timeTaken, 0, 10); // After 30 seconds, lose 10 points only
        return (int)timeTaken;
    }

    public bool IsPaused()
    {
        if(GameMenuManager._Instance == null) return false;
        return GameMenuManager._Instance.IsPaused();
    }
}