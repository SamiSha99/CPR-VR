using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicGoal : Quest.QuestGoal
{   
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onTalking += OnTalkingRecieved;
        Util.GetXREvents().EnableMicRecording();
        _GoalUIType = GoalUIType.GUIT_ProgressBar;
    }
    private void OnTalkingRecieved(GameObject o, GameObject instigator, float talkAmount)
    {
        if (objectiveNameList.Count > 0 && !objectiveNameList.Contains(o.name)) return;
        currentAmount += talkAmount;
        Evaluate();
    }
    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onTalking -= OnTalkingRecieved;
        Util.GetXREvents().DisableMicRecording();
    }
}
