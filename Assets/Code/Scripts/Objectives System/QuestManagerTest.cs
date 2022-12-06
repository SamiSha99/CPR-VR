using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManagerTest : MonoBehaviour
{
    public Quest quest;

    // To-do add a new quest field in the next quest should solve our issue, i hope
    void Start()
    {
        quest.Initialize();
        quest.questCompleted.AddListener(OnQuestCompleted);
        Debug.Log("Begin quest => \"" + quest.information.name + "\"| desc: " + quest.information.desc);
    }

    private void OnQuestCompleted(Quest q)
    {
        Debug.Log("Quest compelted! =>" + q.information.name);
    }
}
