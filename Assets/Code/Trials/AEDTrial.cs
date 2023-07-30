using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AEDTrial : MonoBehaviour
{
    private int appliedPatches;
    public AudioClip beepBeep, aedShockEffect;
    public PlayAudioAtLocation localizedAudioPlayer;
    public UnityEvent _ButtonEnabled;
    public bool isAEDEnabled;

    public void OnAEDTurnedOn()
    {
        isAEDEnabled = true;
    }

    // Handled in inspector!!!
    public void OnPatchApplied()
    {
        appliedPatches++;
        if(appliedPatches >= 2) OnAllPatchesApplied();
    }

    void OnAllPatchesApplied()
    {
        //AudioSource.PlayClipAtPoint(analyzingNow, transform.position);
        localizedAudioPlayer?.TriggerAudio("VA.analyzing_now");
        //QuestManager._Instance.ToggleTimer(false);
        Util.Invoke(this, () => BeepBeep(), 1.5f);
        Util.Invoke(this, () => BeepBeep(), 3.0f);
        Util.Invoke(this, () => BeepBeep(), 4.5f);
        Util.Invoke(this, () => OnWaitingForAED(), 5.0f);
    }

    void OnWaitingForAED()
    {
        QuestManager._Instance.CompleteCommandGoal("Wait_for_AED_Goal_Command");
        Util.Invoke(this, () => OnShockIsRequired(), 0.5f);
    }

    void BeepBeep() => AudioSource.PlayClipAtPoint(beepBeep, transform.position);
    void OnShockIsRequired()
    {
        localizedAudioPlayer?.TriggerAudio("VA.shock_required");
        Util.GetXREvents().EnableMicRecording();
        Util.Invoke(this, () => OnButtonEnabled(), LocalizationHelper.UsingLanguage("ar") ? 4.35f : 3.25f);
    }

    void OnButtonEnabled()
    {
        //QuestManager._Instance.ToggleTimer(true);
        localizedAudioPlayer?.TriggerAudio("VA.press_button");
        _ButtonEnabled?.Invoke();
    }
    // Interacted with the button -> apply shock
    public void OnButtonPressed()
    {
        AudioSource.PlayClipAtPoint(aedShockEffect, transform.position);
        //Util.Invoke(this, () => QuestManager._Instance.CompleteCommandGoal("Press_Shock_Button_Goal_Command"), 3.0f);
        Util.Invoke(this, () => { 
            QuestManager qm = QuestManager._Instance;
            if(!qm.IsQuestGoalCompleted("Said_Clear"))
            {
                GameManager._Instance.AddExamPenalty("ExamPenalty.NotSayingClear", 10.0f);
                QuestManager._Instance.AddQuestToRetry();
            }
            qm.CompleteCommandGoal("Press_Shock_Button_Goal_Command");
            qm.ForceCompleteQuest();
        }, 3.0f);
        
        Reset();
    }

    public void Reset()
    {
        isAEDEnabled = false;
        appliedPatches = 0;
    }
}
