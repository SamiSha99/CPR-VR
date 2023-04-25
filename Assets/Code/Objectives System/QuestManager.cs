using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager _Instance;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private GameObject questGoalList;
    [SerializeField] private GameObject questGoalCellPrefab;
    [Header("Data")]
    [SerializeField] static public Quest activeQuest;
    [Header("Sound")]
    [SerializeField] private AudioClip checkmarkSound;
    [SerializeField] private AudioClip writingSound;
    [Header("Other")]
    [Tooltip("When level is loaded, start this quest immediatly.")]
    public Quest onLoadQuest;
    public GameEventCommand onQuestBegin, onQuestCompleted, onQuestGoalCompleted;
    public bool debugging;

    private float questCurrentTime, accumulatedScorePenalty;
    private bool isQuestTimePaused;

    const string GAMEOBJECT_NAME_GOAL_TITLE = "Goal Text";
    const string GAMEOBJECT_NAME_GOAL_VALUE = "Goal Value";

    void Awake() => _Instance = this;
    void Start()
    {
        SetTextToDefault();
        BeginQuest(onLoadQuest);
    }
    void Update() 
    {
        activeQuest?.QuestUpdate();
        if(!isQuestTimePaused)
            questCurrentTime += Time.deltaTime;
    }
    public void BeginQuest(Quest q)
    {
        if(q == null) return;

        isQuestTimePaused = false;
        questCurrentTime = 0;
        
        activeQuest = q.Initialize(OnQuestCompleted, OnUpdateGoalProgress);
        onQuestBegin?.TriggerEvent(activeQuest.questCommand + Quest.QUEST_BEGIN_COMMAND);

        LocalizationHelper.LocalizeTMP(q.information.name, questName);
        LocalizationHelper.LocalizeTMP(q.information.description, questDescription);
        
        if(questGoalList == null || questGoalCellPrefab == null) return;

        for(int i = 0; i < q.goals.Count; i++)
        {
            if(q.goals[i]._GoalUIType == Quest.QuestGoal.GoalUIType.GUIT_None) continue;

            GameObject p = Instantiate(questGoalCellPrefab, questGoalList.transform);
            p.name = "Quest Goal " + (i + 1);

            TextMeshProUGUI tmp = p.transform.FindComponent<TextMeshProUGUI>(GAMEOBJECT_NAME_GOAL_TITLE);            
                
            LocalizationHelper.LocalizeTMP(q.goals[i].GetDescription(), tmp);

            // Values
            GameObject valueObject = p.transform.Find(GAMEOBJECT_NAME_GOAL_VALUE).gameObject;
            // Numbers
            TextMeshProUGUI textMesh = valueObject.transform.FindComponent<TextMeshProUGUI>("Amount Value");
            LocalizationHelper.LocalizeTMP(q.goals[i].currentAmount + "/" + q.goals[i].requiredAmount, textMesh);
            // Checkbox
            GameObject checkboxObject = valueObject.transform.Find("Checkbox").gameObject;
            // Progress
            GameObject progressObject = valueObject.transform.Find("Progress").gameObject;
            
            textMesh.gameObject?.SetActive(false);
            checkboxObject?.SetActive(false);
            progressObject?.SetActive(false);
            
            switch(q.goals[i]._GoalUIType)
            {
                case Quest.QuestGoal.GoalUIType.GUIT_Default:
                default:
                    textMesh.gameObject.SetActive(true);
                    break;
                case Quest.QuestGoal.GoalUIType.GUIT_Checkbox:
                    checkboxObject.SetActive(true);
                    checkboxObject.GetComponent<Checkbox>()?.CheckmarkBox(false);
                    break;
                case Quest.QuestGoal.GoalUIType.GUIT_ProgressBar:
                    progressObject.SetActive(true);
                    progressObject.GetComponent<ProgressBar>()._slider.value = 0;
                    break;
                case Quest.QuestGoal.GoalUIType.GUIT_None:
                    break;
            }
        }
    }
    private void OnQuestCompleted(Quest q)
    {
        Print("[QUEST COMPLETED] => \"" + q.information.name + "\"");
        onQuestCompleted?.TriggerEvent(q.questCommand + Quest.QUEST_COMPLETE_COMMAND);
        if(!GameManager._Instance.isExam)
            AudioSource.PlayClipAtPoint(writingSound, transform.position);
        
        GameManager gm = GameManager._Instance;
        gm.AdjustScore(questCurrentTime, q.averageTime, accumulatedScorePenalty);
        accumulatedScorePenalty = 0;

        Util.Invoke(this, () => OnPostQuestComplete(q), GameManager._Instance.isExam ? 0.5f : 3.0f);
    }
    private void OnPostQuestComplete(Quest q)
    {
        CleanUpGoalList();
        //if (q.nextQuest != null) BeginQuest(q.nextQuest);
        if(GameManager._Instance.isExam)
            GameManager._Instance.InstigateNextExamObject();
        else
            GameManager._Instance.InstigateNextTutorialObject();
    }
    // Updates the cells that is relevant on the goal via passed QuestGoal script using the index
    private void OnUpdateGoalProgress(Quest.QuestGoal goal)
    {
        List<Transform> transformList = new List<Transform>();
        Transform[] transformArr;

        if(goal._GoalUIType == Quest.QuestGoal.GoalUIType.GUIT_None) return; // Don't update

        transformArr = Util.GetChildren(questGoalList.transform, transformList, false).ToArray();
        int index = goal.index;

        if(index >= transformArr.Length)
        {
            Print("Cannot update \"Goal Progress\" index out of bounds! Did you set the ID correctly?");
            return;
        }
        
        TextMeshProUGUI goalTextCell = transformArr[index].FindComponent<TextMeshProUGUI>(GAMEOBJECT_NAME_GOAL_TITLE);
        GameObject goalValueCell = transformArr[index].Find(GAMEOBJECT_NAME_GOAL_VALUE).gameObject;

        switch(goal._GoalUIType)
        {
            case Quest.QuestGoal.GoalUIType.GUIT_Default:
            default:
                TextMeshProUGUI goalAmountCell = goalValueCell.transform.FindComponent<TextMeshProUGUI>("Amount Value");
                goalAmountCell.text = goal.currentAmount + "/" + goal.requiredAmount;
                goalAmountCell.color = goal.completed ? Color.green : Color.white;
                break;
            case Quest.QuestGoal.GoalUIType.GUIT_Checkbox:
                goalValueCell.transform.FindComponent<Checkbox>("Checkbox")?.CheckmarkBox(goal.completed);
                break;
            case Quest.QuestGoal.GoalUIType.GUIT_ProgressBar:
                float normalized = goal.currentAmount/goal.requiredAmount;
                goalValueCell.transform.FindComponent<ProgressBar>("Progress")?.SetProgressBar(normalized);
                break;
            case Quest.QuestGoal.GoalUIType.GUIT_None:
                break;
        }
        if(goal.completed) 
        {
            goalTextCell.color = Color.green;
            onQuestGoalCompleted?.TriggerEvent(goal.goalCompletedCommand + Quest.QuestGoal.QUEST_GOAL_COMPLETE_COMMAND);
            if(!goal.silent)
                AudioSource.PlayClipAtPoint(checkmarkSound, transform.position);
        }
        else
            goalTextCell.color = Color.white;
    }
    private void CleanUpGoalList()
    {
        while(questGoalList.transform.childCount > 0) DestroyImmediate(questGoalList.transform.GetChild(0).gameObject);
        SetTextToDefault();
    }
    private void SetTextToDefault()
    {
        LocalizationHelper.SetLanguage("ar");
        LocalizationHelper.LocalizeTMP("QuestUI.TitleEmpty", questName);
        LocalizationHelper.LocalizeTMP("QuestUI.DescriptionEmpty", questDescription);
    }
    public void ForceCompleteGoal(int index)
    {
        if(activeQuest == null) return;
        activeQuest.goals[index].Skip();
    }
    // Complete CommandGoal classes by comparing the string sent.
    public void CompleteCommandGoal(string cmd)
    {
        CommandGoal cg;
        foreach (Quest.QuestGoal goal in activeQuest.goals)
        {
            if(goal.GetType() != typeof(CommandGoal)) continue;
            cg = (CommandGoal)goal;
            cg.OnRecieveCommand(cmd);
        }
    }
    public void ForceCompleteQuest()
    {
        if(!IsQuestActive()) return;
        foreach (Quest.QuestGoal goal in activeQuest.goals) goal.Skip();
    }
    public bool IsQuestActive()
    {
        return QuestManager.activeQuest != null;
    }

    public bool IsQuestGoalCompleted(string goal_complete_command)
    {
        if(!IsQuestActive()) return false;

        foreach(Quest.QuestGoal g in activeQuest.goals)
        {
            if(g.goalCompletedCommand == "???") continue; // not even set!!!!
            if(g.goalCompletedCommand != goal_complete_command) continue;
            return g.completed;
        }
        return false;
    }

    public bool IsQuestType(string questTitle)
    {
        return IsQuestActive() && activeQuest.information.name == questTitle;
    }
    public void ForceUpdateGoal(Quest.QuestGoal g) => OnUpdateGoalProgress(g);

    public void OnPenalty(float amount)
    {
        accumulatedScorePenalty += amount;
    }

    public void ToggleTimer(bool _enabled) => isQuestTimePaused = !_enabled;
    
    private void Print(string s)
    {
        if(!debugging) return;
        Util.Print<QuestManager>(s);
    }
}
