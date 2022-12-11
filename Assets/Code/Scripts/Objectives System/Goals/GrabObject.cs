using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : Quest.QuestGoal
{
    [Tooltip("The exact name of the object in the hierarchy.")]
    public string objectName;
    [Tooltip("Will remove completion if dropped.")]
    public bool shouldHold;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemGrabbed += OnGrabbingObject;
    }

    private void OnGrabbingObject(GameObject o, bool dropped)
    {
        if(o.name != objectName) return;
        if(shouldHold && dropped)
            currentAmount--;
        else 
            currentAmount++;
        Evaluate(!shouldHold);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemGrabbed -= OnGrabbingObject;
    }
}
