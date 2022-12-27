using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchObject : Quest.QuestGoal
{
    [Tooltip("The exact name of the object in the hierarchy, requires to have any XR Interactor type, AS LONG AS YOU CAN TOUCH IT!")]
    public string objectName;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemTouched += OnTouchingObject;
    }

    private void OnTouchingObject(GameObject o, bool untouched)
    {
        if (o.name != objectName) return;
        if(untouched) return;
        currentAmount++;
        Evaluate();
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemTouched -= OnTouchingObject;
    }
}
