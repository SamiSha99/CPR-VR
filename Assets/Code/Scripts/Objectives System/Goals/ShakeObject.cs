using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject : Quest.QuestGoal
{
    [HideInInspector] public float accumulatedShake;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemShaked += OnShakeObject;
        accumulatedShake = 0;
    }

    private void OnShakeObject(GameObject o, GameObject instigator, float shakeAmount)
    {
        if (!objectiveNameList.Contains(o.name)) return;
        accumulatedShake += shakeAmount;
        currentAmount = Mathf.FloorToInt(accumulatedShake);
        Evaluate();
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemShaked -= OnShakeObject;
    }
}
