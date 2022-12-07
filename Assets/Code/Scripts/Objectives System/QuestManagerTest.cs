using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManagerTest : MonoBehaviour
{
    public Quest quest;
    // To-do add a new quest field in the next quest should solve our issue, i hope
    void Awake()
    {
        quest.Initialize(OnQuestCompleted);
        Debug.Log("[BEGIN QUEST] => \"" + quest.information.name + "\"| desc: " + quest.information.desc);
    }

    private void OnQuestCompleted(Quest q)
    {
        Debug.LogFormat("[QUEST COMPLETED] => \"" + q.information.name + "\"");
    }
}
