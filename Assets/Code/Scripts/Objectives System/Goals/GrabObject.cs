using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : Quest.QuestGoal
{
    [Tooltip("The exact name of the object in the hierarchy, requires to have XRGrabInteractable.")]
    public string objectName;
    [Tooltip("Set true if we want to remove progress if dropped.")]
    public bool shouldHold;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemGrabbed += OnGrabbingObject;
    }

    private void OnGrabbingObject(GameObject o, GameObject instigator, bool dropped)
    {
        if(o.name != objectName) return;
        
        if(shouldHold && dropped)
            currentAmount--;
        else 
            currentAmount++;

        //GlobalHelper.Print<GrabObject>("Grabbed, touched, whateve" + currentAmount);
        Evaluate(!shouldHold);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemGrabbed -= OnGrabbingObject;
    }
}
