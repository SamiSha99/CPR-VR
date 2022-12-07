using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : Quest.QuestGoal
{
    [Tooltip("The exact name of the object in the hierarchy.")]
    public string objectName;
    public override void Initialize()
    {
        base.Initialize();
        XRPlayer.onItemGrabbed += OnGrabbingObject;
    }

    public override string GetDescription()
    {
        return $"Grab a/an {objectName}";
    }

    private void OnGrabbingObject(GameObject o, bool dropped)
    {
        if(!dropped && o.name == objectName)
        {
            currentAmount++;
            Evaluate();
        }
    }

    protected override void CleanUp()
    {
        XRPlayer.onItemGrabbed -= OnGrabbingObject;
    }
}
