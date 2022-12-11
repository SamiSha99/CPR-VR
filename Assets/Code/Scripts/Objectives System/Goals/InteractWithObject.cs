using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractWithObject : Quest.QuestGoal
{
    [Tooltip("The exact name of the object in the hierarchy.")]
    public string objectName;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemInteracted += OnInteractingObject;
    }

    private void OnInteractingObject(GameObject o)
    {
        if (o.name != objectName) return;
        currentAmount++;
        Evaluate();
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemInteracted -= OnInteractingObject;
    }
}
