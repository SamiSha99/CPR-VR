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
    [SerializeField] private GameObject questGoalPrefab;
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
        q.Initialize(OnQuestCompleted);
        Print("[BEGIN QUEST] => \"" + q.information.name + "\"Description: " + q.information.desc);
        if (debugging)
            for (int i = 0; i < q.goals.Count; i++)
                Print("[QUEST GOAL " + (i + 1) + "] => \"" + q.goals[i].GetDescription() + " | [REQUIRED AMOUNT] => " + q.goals[i].requiredAmount);
        questName.text = q.information.name;
        questDescription.text = q.information.desc;
        foreach(Quest.QuestGoal g in q.goals)
        {
            GameObject p = Instantiate(questGoalPrefab, questGoalList.transform);
            p.transform.SetParent(questGoalList.transform);
            p.transform.Find("Goal Text").gameObject.GetComponent<TextMeshProUGUI>().text = g.GetDescription();
            p.transform.Find("Goal Amount").gameObject.GetComponent<TextMeshProUGUI>().text = g.requiredAmount.ToString();
        }
    }

    private void OnQuestCompleted(Quest q)
    {
        Print("[QUEST COMPLETED] => \"" + q.information.name + "\"");
        if (q.nextQuest != null)
        {
            BeginQuest(q.nextQuest);
        }
    }

    private void Print(string s)
    {
        if(!debugging) return;
        GlobalHelper.Print(this, s);
    }
}
