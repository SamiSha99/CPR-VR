using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManagerTest : MonoBehaviour
{
    public Quest quest;
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
