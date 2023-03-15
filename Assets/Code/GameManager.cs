using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance;
    public List<Object> _TutorialQuestsLine;
    private List<Object> default_TutotrialQuestsLine;
    [Header("EXAM MANAGER")]
    // Exam
    public bool isExam, completed;
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
        isExam = SettingsUtility.IsChecked("isExam", false);

        default_TutotrialQuestsLine = new List<Object>(_TutorialQuestsLine);
        default_ExamQuestsLine = new List<Quest>(_ExamQuestsLine);
        _RepeatableAmount = Mathf.Max(1, _RepeatableAmount);

        if(isExam)
        {
            BuildExam();
            InstigateNextExamObject();
        }
        else
            InstigateNextTutorialObject();
    }

    void Update()
    {
        
    }

    public void InstigateNextTutorialObject()
    {
        if(OnGameComplete()) return;
        
        switch(_TutorialQuestsLine[0])
        {
            case Quest q:
                QuestManager._Instance.BeginQuest(q);
                break;
            case AudioClip ac:
                Util.PlayClipAt(ac, Util.GetPlayer().GetPlayerCameraObject().transform.position, 1.0f, Util.GetPlayer().GetPlayerCameraObject());
                Util.Invoke(this, () => InstigateNextTutorialObject(), ac.length + 0.25f);
                break;
        }
        _TutorialQuestsLine.RemoveAt(0);
    }

    public void BuildExam()
    {

    }

    public void InstigateNextExamObject()
    {
        if(OnGameComplete()) return;
        
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
        
        Util.Invoke(this, () => Util.LoadMenu(), 10.0f);
        
        // TO-DO
        // Show Score
        // Animation
        // Then leave in 10 seconds?
        return true;
    }
}
