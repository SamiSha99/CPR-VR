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
    [SerializeField] private Quest activeQuest;
    [HeaderAttribute("Other")]
    [Tooltip("When level is loaded, start this quest immediatly.")]
    public Quest onLoadQuest;
    public bool debugging;

    void Awake()
    {
        if(onLoadQuest != null) BeginQuest(onLoadQuest);
    }

    private void BeginQuest(Quest q)
    {
        q.Initialize(OnQuestCompleted, OnUpdateGoalProgress);
        
        if (debugging)
        {
            Print("[BEGIN QUEST] => \"" + q.information.name + "\"Description: " + q.information.desc);
            for (int i = 0; i < q.goals.Count; i++)
                Print("[QUEST GOAL " + (i + 1) + "] => \"" + q.goals[i].GetDescription() + " | [REQUIRED AMOUNT] => " + q.goals[i].requiredAmount);
        }

        questName.text = q.information.name;
        questDescription.text = q.information.desc;

        if(questGoalList == null || questGoalCellPrefab == null) return;

        for(int i = 0; i < q.goals.Count; i++)
        {
            GameObject p = Instantiate(questGoalCellPrefab, questGoalList.transform);
            p.name = "Quest Goal " + (i + 1);
            p.transform.FindComponent<TextMeshProUGUI>("Goal Text").text = q.goals[i].GetDescription();
            p.transform.FindComponent<TextMeshProUGUI>("Goal Amount").text = q.goals[i].currentAmount + "/" + q.goals[i].requiredAmount;
        }
    }

    private void OnQuestCompleted(Quest q)
    {
        Print("[QUEST COMPLETED] => \"" + q.information.name + "\"");
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

        TextMeshProUGUI goalTextCell = transformArr[index].FindComponent<TextMeshProUGUI>("Goal Text"), goalAmountCell = transformArr[index].FindComponent<TextMeshProUGUI>("Goal Amount");
        goalAmountCell.text = goal.currentAmount + "/" + goal.requiredAmount;
        if(goal.completed)
        {
            goalTextCell.color = Color.green;
            goalAmountCell.color = Color.green;
        }
        else
        {
            goalTextCell.color = Color.white;
            goalAmountCell.color = Color.white;
        }
    }

    private void CleanUpGoalList()
    {
        while(questGoalList.transform.childCount > 0) DestroyImmediate(questGoalList.transform.GetChild(0).gameObject);
        questName.text = "No Objectives";
        questDescription.text = "You are all caught up! Good job!";
    }

    private void Print(string s)
    {
        if(!debugging) return;
        GlobalHelper.Print<QuestManager>(s);
    }
}
