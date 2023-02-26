using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InteractWithObject : Quest.QuestGoal
{
    [Tooltip("Set true if we are required to be holding down the button all time.")]
    public bool shouldHold;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemInteracted += OnInteractingObject;
    }

    private void OnInteractingObject(GameObject o, GameObject instigator, bool uninteracted)
    {
        if (!objectiveNameList.Contains(o.name)) return;
        currentAmount = shouldHold && uninteracted ? (currentAmount - 1) : (currentAmount + 1);
        Evaluate(!shouldHold);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemInteracted -= OnInteractingObject;
    }
}
