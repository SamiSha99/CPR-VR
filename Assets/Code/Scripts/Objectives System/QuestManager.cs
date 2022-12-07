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
        onLoadQuest.Initialize(OnQuestCompleted);
        Print("[BEGIN QUEST] => \"" + onLoadQuest.information.name + "\"Description: " + onLoadQuest.information.desc);
        if(debugging)
            for(int i = 0; i < onLoadQuest.goals.Count; i++) 
                Print("[QUEST GOAL " + (i + 1) +"] => \"" + onLoadQuest.goals[i].GetDescription() + " | [REQUIRED AMOUNT] => " + onLoadQuest.goals[i].requiredAmount);
        
    }

    private void OnQuestCompleted(Quest q)
    {
        Print("[QUEST COMPLETED] => \"" + q.information.name + "\"");
    }

    private void Print(string s)
    {
        if(!debugging) return;
        Debug.Log(s);
    }
}
