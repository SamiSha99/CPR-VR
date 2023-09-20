using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicGoal : Quest.QuestGoal
{   
    [Tooltip("Lose every second amount of points, this to encourage louder microphone talking.")]
    [Min(0)]
    public float micDecayAmount = 1.0f;
    [Tooltip("Multiply the recieved talkAmount value in the OnTalkingRecieved(...).\nMakes goal completion more forgiving... or harder...")]
    [Min(0.01f)]
    public float micSensitivityMultiplier = 1.0f;
    [Tooltip("If checked will immediatly start recording mic, otherwise, it might be handled elsewhere.")]
    public bool startListening = true;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onTalking += OnTalkingRecieved;
        if(startListening)
            Util.GetXREvents().EnableMicRecording();
        _GoalUIType = GoalUIType.GUIT_ProgressBar;
    }
    private void OnTalkingRecieved(float talkAmount, float talkAmountNormalized)
    {
        if(talkAmountNormalized < 1.0f) return;
        if(!IsValidToEvaluate()) return;
        
        talkAmount *= micSensitivityMultiplier;
        currentAmount += talkAmount;
        Evaluate();
    }
    public override void QuestGoalUpdate()
    {
        if(micDecayAmount <= 0) return;
        if(completed) return;
        XREvents xrE = Util.GetXREvents();
        if(xrE != null && !xrE.isTalking)
            currentAmount = Mathf.Max(currentAmount - Time.deltaTime * micDecayAmount, 0.0f);
        QuestManager._Instance.ForceUpdateGoal(this);
    }
    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onTalking -= OnTalkingRecieved;
        Util.GetXREvents()?.DisableMicRecording();
    }
}
