using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [HeaderAttribute("UI")]
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private GameObject questGoalList;
    [SerializeField] private GameObject questGoalCellPrefab;
    [HeaderAttribute("Data")]
    [SerializeField] static public Quest activeQuest;
    [HeaderAttribute("Sound")]
    [SerializeField] private AudioClip checkmarkSound;
    [SerializeField] private AudioClip writingSound;
    [HeaderAttribute("Other")]
    [Tooltip("When level is loaded, start this quest immediatly.")]
    public Quest onLoadQuest;
    public GameEventCommand onQuestBegin, onQuestCompleted, onQuestGoalCompleted;
    public bool debugging;

    const string GAMEOBJECT_NAME_GOAL_TITLE = "Goal Text";
    const string GAMEOBJECT_NAME_GOAL_VALUE = "Goal Value";

    void Start() => BeginQuest(onLoadQuest);
    private void BeginQuest(Quest q)
    {
        if(q == null) return;
        
        activeQuest = q.Initialize(OnQuestCompleted, OnUpdateGoalProgress);
        //Print("Begin Quest Command? => " + activeQuest.beginQuestCommand);
        onQuestBegin?.TriggerEvent(activeQuest.questCommand + Quest.QUEST_BEGIN_COMMAND);
        if (debugging)
        {
            Print("[BEGIN QUEST] => \"" + q.information.name + "\"Description: " + q.information.description);
            for (int i = 0; i < q.goals.Count; i++)
                Print("[QUEST GOAL " + (i + 1) + "] => \"" + q.goals[i].GetDescription() + " | [REQUIRED AMOUNT] => " + q.goals[i].requiredAmount);
        }

        questName.text = q.information.name;
        questDescription.text = q.information.description;

        if(questGoalList == null || questGoalCellPrefab == null) return;

        for(int i = 0; i < q.goals.Count; i++)
        {
            GameObject p = Instantiate(questGoalCellPrefab, questGoalList.transform);
            p.name = "Quest Goal " + (i + 1);
            p.transform.FindComponent<TextMeshProUGUI>(GAMEOBJECT_NAME_GOAL_TITLE).text = $"{i + 1}) " + q.goals[i].GetDescription();

            // Values
            GameObject valueObject = p.transform.Find(GAMEOBJECT_NAME_GOAL_VALUE).gameObject;
            // Numbers
            TextMeshProUGUI textMesh = valueObject.transform.FindComponent<TextMeshProUGUI>("Amount Value");
            textMesh.text = (q.goals[i].currentAmount + "/" + q.goals[i].requiredAmount);
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
            }
        }
    }
    private void OnQuestCompleted(Quest q)
    {
        Print("[QUEST COMPLETED] => \"" + q.information.name + "\"");
        onQuestCompleted?.TriggerEvent(q.questCommand + Quest.QUEST_COMPLETE_COMMAND);
        AudioSource.PlayClipAtPoint(writingSound, transform.position);
        GlobalHelper.Invoke(this, () => OnPostQuestComplete(q), 3.0f);
    }
    private void OnPostQuestComplete(Quest q)
    {
        CleanUpGoalList();
        if (q.nextQuest != null) BeginQuest(q.nextQuest);
    }
    // Updates the cells that is relevant on the goal via index of how it was initialized
    private void OnUpdateGoalProgress(Quest.QuestGoal goal)
    {
        List<Transform> transformList = new List<Transform>();
        Transform[] transformArr;
        
        transformArr = GlobalHelper.GetChildren(questGoalList.transform, transformList, false).ToArray();
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
                goalValueCell.transform.FindComponent<ProgressBar>("Progress")?.UpdateProgressBar(normalized);
                break;
        }
        if(goal.completed) 
        {
            goalTextCell.color = Color.green;
            onQuestGoalCompleted?.TriggerEvent(goal.goalCompletedCommand);
            AudioSource.PlayClipAtPoint(checkmarkSound, transform.position);
        }
        else
            goalTextCell.color = Color.white;
    }
    private void CleanUpGoalList()
    {
        while(questGoalList.transform.childCount > 0) DestroyImmediate(questGoalList.transform.GetChild(0).gameObject);
        questName.text = "No Objectives";
        questDescription.text = "You are all caught up! Good job!";
    }
    public static void ForceCompleteGoal(int index)
    {
        if(activeQuest == null) return;
        activeQuest.goals[index].Skip();
    }
    public static void ForceCompleteQuest()
    {
        foreach (Quest.QuestGoal goal in activeQuest.goals) goal.Skip();
    }
    private void Print(string s)
    {
        if(!debugging) return;
        GlobalHelper.Print<QuestManager>(s);
    }
}
