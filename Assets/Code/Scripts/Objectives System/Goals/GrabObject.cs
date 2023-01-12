using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : Quest.QuestGoal
{
    [Tooltip("Set true if we want to remove progress if dropped.")]
    public bool shouldHold;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemGrabbed += OnGrabbingObject;
    }

    private void OnGrabbingObject(GameObject o, GameObject instigator, bool dropped)
    {
        if(objectiveNameList.Contains(o.name)) return;
        currentAmount = shouldHold && dropped ? (currentAmount - 1) : (currentAmount + 1);
        Evaluate(!shouldHold);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemGrabbed -= OnGrabbingObject;
    }
}
