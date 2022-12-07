using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManagerTest : MonoBehaviour
{
    public Quest quest;
    public GameObject ball;
    // To-do add a new quest field in the next quest should solve our issue, i hope
    void Awake()
    {
        quest.Initialize(OnQuestCompleted);
        Debug.Log("Begin quest => \"" + quest.information.name + "\"| desc: " + quest.information.desc);
        //TestGrabTheBallLMAO();
    }

    private void OnQuestCompleted(Quest q)
    {
        Debug.Log("Quest completed! =>" + q.information.name);
    }

    //public void TestGrabTheBallLMAO()
    //{
    //    XRPlayer.ActionCallTestLMAO(ball);
    //}
}
