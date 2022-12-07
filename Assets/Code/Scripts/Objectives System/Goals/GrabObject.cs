using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : Quest.QuestGoal
{
    [Tooltip("The exact name of the object in the hierarchy.")]
    public string objectName;
    [Tooltip("Will remove completion if dropped.")]
    public bool disqualifyOnDropping;
    public override void Initialize()
    {
        base.Initialize();
        XRPlayer.onItemGrabbed += OnGrabbingObject;
    }

    public override string GetDescription()
    {
        return $"Hold the {objectName}";
    }

    private void OnGrabbingObject(GameObject o, bool dropped)
    {
        if(o.name != objectName) return;
        if(dropped)
            currentAmount--;
        else 
            currentAmount++;
        Evaluate(!disqualifyOnDropping);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XRPlayer.onItemGrabbed -= OnGrabbingObject;
    }
}
