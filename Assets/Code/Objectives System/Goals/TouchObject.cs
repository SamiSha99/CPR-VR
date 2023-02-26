using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchObject : Quest.QuestGoal
{
    [Tooltip("Can only be triggered by the player's touch, if not, then this is not subscribed to XREvents (player's scope) and instead handled somewhere else.")]
    public bool isPlayerTouch = true;
    public override void Initialize()
    {
        base.Initialize();
        if (isPlayerTouch)
            XREvents.onItemTouched += OnTouchingObject;
        else
            QuestEventTouch.onItemTouched += OnTouchingObject;
    }

    private void OnTouchingObject(GameObject o, GameObject instigator, bool untouched)
    {
        if (!objectiveNameList.Contains(o.name)) return;
        if (untouched) return;
        currentAmount++;
        Evaluate();
    }

    public override void CleanUp()
    {
        base.CleanUp();
        if (isPlayerTouch)
            XREvents.onItemTouched -= OnTouchingObject;
        else
            QuestEventTouch.onItemTouched -= OnTouchingObject;
    }
}
