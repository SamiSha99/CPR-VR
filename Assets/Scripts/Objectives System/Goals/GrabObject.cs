using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GrabObject : Quest.QuestGoal
{
    public string objectName;
    public override void Initialize()
    {
        base.Initialize();
        // sub the action here!!!
    }

    public override string GetDescription()
    {
        return $"Grab a/an {objectName}";
    }

    private void OnGrabbingObject(GameObject o)
    {
        if(o.name == objectName)
        {
            currentAmount++;
            Evaluate();
        }
    }
}
