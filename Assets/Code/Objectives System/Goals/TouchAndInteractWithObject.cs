using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchAndInteractWithObject : Quest.QuestGoal
{
    [Tooltip("Toggle whether this should evaluate interactions.")]
    public bool allowInteracting = true;
    [Tooltip("Toggle whether this should evaluate touches.")]
    public bool allowTouching = true; 
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemInteracted += OnInteractingObject;
        XREvents.onItemTouched += OnTouchingObject;
    }

    private void OnInteractingObject(GameObject o, GameObject instigator, bool uninteracted)
    {
        if (objectiveNameList.Count > 0 && !objectiveNameList.Contains(o.name)) return;
        if(uninteracted) return;
        currentAmount++;
        Evaluate();
    }

    private void OnTouchingObject(GameObject o, GameObject instigator, bool untouched)
    {
        if (objectiveNameList.Count > 0 && !objectiveNameList.Contains(o.name)) return;
        if (untouched) return;
        currentAmount++;
        Evaluate();
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemInteracted -= OnInteractingObject;
        XREvents.onItemTouched -= OnTouchingObject;
    }
}
