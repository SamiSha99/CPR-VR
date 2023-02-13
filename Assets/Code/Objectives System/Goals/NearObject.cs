using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearObject : Quest.QuestGoal
{
    [Tooltip("Set true if we want to remove progress if dropped.")]
    public float _NearbyRange = 1.0f;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemGrabbed += OnGrabbingObject;
    }

    private void OnGrabbingObject(GameObject o, GameObject instigator, bool dropped)
    {
        if(!objectiveNameList.Contains(o.name)) return;
        currentAmount = requiredAmount;
        Evaluate();
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemGrabbed -= OnGrabbingObject;
    }
}
