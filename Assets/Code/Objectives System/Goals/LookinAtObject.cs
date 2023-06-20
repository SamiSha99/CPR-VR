using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookinAtObject : Quest.QuestGoal
{   
    [Tooltip("The look range needed, the line will be as tall as the specfied range.")]
    public float lookRange = 5;
    private PlayerLookAtObject lookScript;
    private bool lookingAtTarget;
    [Tooltip("Set currentAmount to 0 on unfocus, will uncomplete if completed.")]
    public bool shouldMaintainLooking;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemLookedAt += OnLookAtObjectRecieved;
        lookScript = Util.GetPlayer().transform.FindComponent<PlayerLookAtObject>("XREvents");
        if(lookScript == null) return;
        lookScript.lookRange = lookRange;
        lookScript.showHelpLine = true;
    }
    // focusTime is the continous focus!!!
    private void OnLookAtObjectRecieved(GameObject o, GameObject instigator, bool lookingAt, float focusTime)
    {
        if(o == null || !IsValidToEvaluate(o.name))
        {
            if(shouldMaintainLooking) currentAmount = 0;
            lookingAtTarget = false;
            SetLookColor(Color.red);
            return;
        }

        SetLookColor(Color.green);
        lookingAtTarget = true;

        currentAmount = (shouldMaintainLooking && !lookingAt ? 0 : (currentAmount + Time.deltaTime));
    
        Evaluate(!shouldMaintainLooking);
    }
    public override void QuestGoalUpdate()
    {
        if(completed) return;
        if(!lookingAtTarget) return;
        QuestManager._Instance.ForceUpdateGoal(this);
    }
    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemLookedAt -= OnLookAtObjectRecieved;
        if(lookScript == null) return;
        lookScript.lookRange = PlayerLookAtObject.DEFAULT_LOOK_RANGE;
        lookScript.showHelpLine = false;
    }

    public void SetLookColor(Color c)
    {
        if(lookScript == null) return;
        lookScript.SetHelperColor(c);
    }
}
