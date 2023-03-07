using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance;
    public List<Quest> _TutorialQuestsLine;
    private List<Quest> default_TutotrialQuestsLine;
    [Header("EXAM MANAGER")]
    // Exam
    public bool isExam;
    [Tooltip("The exam's content step by step, each step is evaulated.")]
    public List<Quest> _ExamQuestsLine; // We do this step by step to examinate how fast/slow and effecient the player is and we add score
    [Tooltip("These tasks are repeated continuously X times, depeding on Repeatable Amount, also evaulated")]
    public List<Quest> _RepeatableQuestLine;
    [Min(1)]
    public int _RepeatableAmount = 1;
    private List<Quest> default_ExamQuestsLine;
    public float score, maxScore;
    void Awake() => _Instance = this;
    void Start()
    {
        BeginTutorial();
        
        isExam = SettingsUtility.IsChecked("isExam", false);
        Util.Print<GameManager>("Is Exam ? => " + isExam);
    }

    public void BeginNextQuest()
    {
        if(_TutorialQuestsLine.Count <= 0)
        {
            OnGameComplete();
            return;
        }

        if(!Util.IsQuestManagerActive()) return;

        QuestManager._Instance.BeginQuest(_TutorialQuestsLine[0]);
        _TutorialQuestsLine.RemoveAt(0);
    }
    void OnGameComplete()
    {
        // TO-DO
        // Show Score
        // Animation
        // Then leave in 10 seconds?
    }

    public void BeginTutorial()
    {
        default_TutotrialQuestsLine = new List<Quest>(_TutorialQuestsLine);
        default_ExamQuestsLine = new List<Quest>(_ExamQuestsLine);
        _RepeatableAmount = Mathf.Max(1, _RepeatableAmount);
        if(_TutorialQuestsLine.Count > 0) BeginNextQuest();
    }
}
