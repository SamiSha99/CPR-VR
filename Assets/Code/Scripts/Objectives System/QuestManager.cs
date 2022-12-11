using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
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
