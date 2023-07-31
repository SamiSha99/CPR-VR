using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance;
    public List<Object> _TutorialQuestsLine;
    private List<Object> default_TutotrialQuestsLine;
    public List<Quest> retryQuests = new List<Quest>();
    // Exam
    [HideInInspector] public bool isExam;
    [HideInInspector] public bool completed;
    [Tooltip("The exam's content step by step, each step is evaulated.")]
    public List<Object> _ExamQuestsLine; // We do this step by step to examinate how fast/slow and effecient the player is and we add score
    public struct ExamPenalty {
        public string penaltyName;
        public float penaltyAmount;
        public ExamPenalty(string penaltyName)
        {
            this.penaltyName = penaltyName;
            penaltyAmount = 0;
        }
        public ExamPenalty(string penaltyName, float penaltyAmount)
        {
            this.penaltyName = penaltyName;
            this.penaltyAmount = penaltyAmount;
        }
    }
    public List<ExamPenalty> _ExamPenalty = new List<ExamPenalty>();
    public int accumulatedPenalties;
    public ExamResults resultCanvas;
    [Tooltip("These tasks are repeated continuously X times, depending on Repeatable Amount, also evaulated")]
    //public List<Object> _RepeatableQuestLine;
    [Min(1)]
    //public int _RepeatableAmount = 1;
    private List<Object> default_ExamQuestsLine;
    public float score, maxScore;
    public GameEventCommand OnModuleProgressed;

    const string TUTORIAL_EVENT = "_Tutorial";
    const string EXAM_EVENT = "_Exam";

    void Awake() => _Instance = this;
    void Start()
    {
        if(Util.IsInMainMenu()) return;
        Init();
    }

    void Init()
    {
        isExam = SettingsUtility.IsChecked(nameof(isExam), false);

        default_TutotrialQuestsLine = new List<Object>(_TutorialQuestsLine);
        default_ExamQuestsLine = new List<Object>(_ExamQuestsLine);
        
        if(isExam)
        {
            if(default_ExamQuestsLine.Count <= 0) return;
            score = maxScore;
            InstigateNextExamObject();
        }
        else
        {
            if(default_TutotrialQuestsLine.Count <= 0) return;
            InstigateNextTutorialObject();
        }
    }

    public void InstigateNextTutorialObject()
    {
        string command = "";

        if(OnGameComplete()) return;
        if(_TutorialQuestsLine.Count > 0)
        {
            switch(_TutorialQuestsLine[0])
            {
                case Quest q:
                    QuestManager._Instance.BeginQuest(q);
                    command = q.questCommand;
                    break;
                case AudioClip ac:
                    GameObject head = Util.GetPlayer().GetPlayerCameraObject();
                    AudioClip localizedAudio = LocalizationHelper.GetAsset<AudioClip>("VA." + ac.name);
                    if(localizedAudio == null) localizedAudio = ac; // cannot be found, use the english one
                    Util.PlayClipAt(localizedAudio, head.transform.position, PlayerPrefs.GetFloat(nameof(SettingsManager.textToSpeechVolume), 1.0f), head);
                    Util.Invoke(this, () => InstigateNextTutorialObject(), localizedAudio.length + 0.25f);
                    command = ac.name;
                    break;
            }
            _TutorialQuestsLine.RemoveAt(0);
        }
        else
        {
            QuestManager._Instance.BeginQuest(retryQuests[0]);
            command = retryQuests[0].questCommand;
            retryQuests.RemoveAt(0);
        }
        OnModuleProgressed.TriggerEvent(command + TUTORIAL_EVENT);
        OnModuleProgressed.TriggerEvent(command);
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
        if(!isExam && retryQuests.Count > 0) return false;

        completed = true;

        if(isExam)
        {
            ShowFinalScore();
            CSVSaver.SaveData(score, _ExamPenalty, Time.timeSinceLevelLoadAsDouble);
        }
        else
            Util.Invoke(this, () => Util.LoadMenu(), 5.0f);
        return true;
    }

    private void ShowFinalScore()
    {
        if(resultCanvas == null) return;
        resultCanvas.ShowFinalScore(score, _ExamPenalty);
    }

    public void AddQuestToRetry(Quest q)
    {
        if(isExam) return;
        if(retryQuests == null || retryQuests.Contains(q)) return;
        retryQuests.Add(q);
    }

    public void AddExamPenalty(string mistakeLocalization, float scorePenalty, bool noPrompt = false)
    {
        if(scorePenalty <= 0) return; // 0? alright
        
        if(!isExam)
        {
            if(!noPrompt) MistakeManager._Instance?.OnMistakeRecieved(mistakeLocalization);
            return; 
        }
        if(_ExamPenalty == null) return;
        
        if(_ExamPenalty.Count <= 0)
        {
            _ExamPenalty.Add(new ExamPenalty(mistakeLocalization, scorePenalty));
            return; 
        }
        
        for(int i = 0; i < _ExamPenalty.Count; i++)
        {
            if(_ExamPenalty[i].penaltyName != mistakeLocalization) continue;
            _ExamPenalty[i] = new ExamPenalty(_ExamPenalty[i].penaltyName, _ExamPenalty[i].penaltyAmount + scorePenalty);
            return;
        }
        
        _ExamPenalty.Add(new ExamPenalty(mistakeLocalization, scorePenalty));
    }
    // after avg, every 2 seconds reduces this score by 1, up to 5.
    const int REDUCE_AFTER_SECONDS = 2;

    public void AdjustScore(float timeTaken, float averageTime)
    {
        if(timeTaken <= averageTime) return;
        AddExamPenalty("ExamPenalty.Time", GetTimePenalty(timeTaken, averageTime), true);
    }

    private int GetTimePenalty(float timeTaken, float averageTime)
    {
        timeTaken -= timeTaken % REDUCE_AFTER_SECONDS; // remove leftovers
        timeTaken -= averageTime;
        timeTaken /= REDUCE_AFTER_SECONDS;
        timeTaken = Mathf.Clamp(timeTaken, 0, 5); // After 30 seconds, lose 5 points only
        return (int)timeTaken;
    }

    public bool IsPaused()
    {
        if(GameMenuManager._Instance == null) return false;
        return GameMenuManager._Instance.IsPaused();
    }
}