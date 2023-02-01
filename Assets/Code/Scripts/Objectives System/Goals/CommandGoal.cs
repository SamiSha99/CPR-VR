using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandGoal : Quest.QuestGoal
{
    const string COMMAND_GOAL_TEMPLATE = "???_Complete_Goal";
    [Header("Call this command to complete")]
    public string GoalCommand = COMMAND_GOAL_TEMPLATE;
    public override void Initialize()
    {
        base.Initialize();
    }
}
