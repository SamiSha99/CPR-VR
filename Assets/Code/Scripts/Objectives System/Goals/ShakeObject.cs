using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject : Quest.QuestGoal
{
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemShaked += OnShakeObject;
        _GoalUIType = GoalUIType.GUIT_ProgressBar;
    }
    private void OnShakeObject(GameObject o, GameObject instigator, float shakeAmount)
    {
        if (!objectiveNameList.Contains(o.name)) return;
        currentAmount += shakeAmount;
        Evaluate();
    }
    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemShaked -= OnShakeObject;
    }
}
