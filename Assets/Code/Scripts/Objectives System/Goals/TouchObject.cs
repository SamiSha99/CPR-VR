using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchObject : Quest.QuestGoal
{
    [Tooltip("The exact name of the object in the hierarchy, requires to have any XR Interactor type, AS LONG AS YOU CAN TOUCH IT!")]
    public string objectName;
    [Tooltip("Can only be triggered by the player's touch, if not, then this is not subscribed to XREvents (player's scope) and instead handled somewhere else.")]
    public bool isPlayerTouch = true;
    public override void Initialize()
    {
        base.Initialize();
        if(isPlayerTouch)
            XREvents.onItemTouched += OnTouchingObject;
        else
            QuestEventTouch.onItemTouched += OnTouchingObject;
    }

    private void OnTouchingObject(GameObject o, bool untouched)
    {
        //GlobalHelper.Print<TouchObject>("o.name =" + o.name + "objectName = " + objectName + "| untouch? =>" + untouched);
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
