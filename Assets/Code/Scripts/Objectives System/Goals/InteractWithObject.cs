using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractWithObject : Quest.QuestGoal
{
    [Tooltip("The exact name of the object in the hierarchy, requires to have XRSimpleInteractable.")]
    public string objectName;
    [Tooltip("Set true if we are required to be holding down the button all time.")]
    public bool shouldHold;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemInteracted += OnInteractingObject;
    }

    private void OnInteractingObject(GameObject o, bool uninteracted)
    {
        if (o.name != objectName) return;
        // will add later
        if(uninteracted) return;
        if(shouldHold && uninteracted)
            currentAmount--;
        else
            currentAmount++;
        Evaluate(!shouldHold);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemInteracted -= OnInteractingObject;
    }
}
