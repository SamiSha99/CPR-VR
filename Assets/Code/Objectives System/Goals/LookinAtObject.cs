using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookinAtObject : Quest.QuestGoal
{   
    public float lookRange = 5;
    private PlayerLookAtObject lookScript;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemLookedAt += OnLookAtObjectRecieved;
        lookScript = Util.GetPlayer().GetComponent<PlayerLookAtObject>();
        if(lookScript == null) return;
        lookScript.lookRange = lookRange;
        lookScript.showHelpLine = true;
    }
    private void OnLookAtObjectRecieved(GameObject o, GameObject instigator, bool lookingAt, float focusTime)
    {
        if (objectiveNameList.Count > 0 && !objectiveNameList.Contains(o.name))
        {
            SetLookColor(Color.red);
            return;
        }
        SetLookColor(Color.green);
        if(!lookingAt) return;
        currentAmount = focusTime;
        Evaluate();
    }
    public override void QuestGoalUpdate()
    {
        if(completed) return;
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
